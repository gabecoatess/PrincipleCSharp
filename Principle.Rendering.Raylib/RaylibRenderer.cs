using System.Collections.Immutable;
using Principle.Rendering.Abstractions;
using Raylib_cs;
using NativeRaylib = Raylib_cs.Raylib;
using NativeRlgl = Raylib_cs.Rlgl;
using NativeRectangle = Raylib_cs.Rectangle;

namespace Principle.Rendering.Raylib;

internal sealed class RaylibRenderer(RaylibContext context) : IRenderer
{
    public RenderResult Submit(RenderTargetHandle target, RenderFrame frame)
    {
        var access = context.CheckRenderAccess();
        if (!access.IsSuccess)
        {
            return access;
        }

        if (!context.Targets.TryGet(target, out var entry))
        {
            return RenderResult.Failure(RenderErrorCode.InvalidHandle, "The render target handle is invalid.");
        }

        var validation = RaylibCommandValidator.Validate(frame);
        if (!validation.IsSuccess)
        {
            return validation;
        }

        try
        {
            if (entry.Kind == RaylibRenderTargetKind.Window)
            {
                NativeRaylib.BeginDrawing();
                Execute(frame);
                NativeRlgl.DrawRenderBatchActive();
            }
            else
            {
                NativeRaylib.BeginTextureMode(entry.RenderTexture);
                try
                {
                    Execute(frame);
                }
                finally
                {
                    NativeRaylib.EndTextureMode();
                }
            }

            return RenderResult.Success();
        }
        catch (Exception exception)
        {
            return RenderResult.Failure(
                RenderErrorCode.BackendFailure,
                "Raylib failed while submitting a render frame.",
                exception.Message);
        }
    }

    public RenderResult<RenderTargetHandle> CreateOffscreenTarget(RenderTargetDescription description)
    {
        var access = context.CheckRenderAccess();
        if (!access.IsSuccess)
        {
            return Failure<RenderTargetHandle>(access.Error!);
        }

        if (description.Width <= 0 || description.Height <= 0)
        {
            return RenderResult<RenderTargetHandle>.Failure(
                RenderErrorCode.ResourceCreationFailed,
                "Offscreen render-target dimensions must be positive.");
        }

        try
        {
            var renderTexture = NativeRaylib.LoadRenderTexture(description.Width, description.Height);
            if (!NativeRaylib.IsRenderTextureValid(renderTexture))
            {
                return RenderResult<RenderTargetHandle>.Failure(
                    RenderErrorCode.ResourceCreationFailed,
                    "Raylib could not create the offscreen render target.");
            }

            return RenderResult<RenderTargetHandle>.Success(
                context.Targets.RegisterOffscreen(renderTexture));
        }
        catch (Exception exception)
        {
            return RenderResult<RenderTargetHandle>.Failure(
                RenderErrorCode.ResourceCreationFailed,
                "Raylib could not create the offscreen render target.",
                exception.Message);
        }
    }

    public RenderResult DestroyRenderTarget(RenderTargetHandle target)
    {
        var access = context.CheckRenderAccess();
        if (!access.IsSuccess)
        {
            return access;
        }

        if (!context.Targets.TryGet(target, out var entry))
        {
            return RenderResult.Failure(RenderErrorCode.InvalidHandle, "The render target handle is invalid.");
        }

        if (entry.Kind == RaylibRenderTargetKind.Window)
        {
            return RenderResult.Failure(
                RenderErrorCode.UnsupportedTargetOperation,
                "The window render target is owned by the render session.");
        }

        try
        {
            NativeRaylib.UnloadRenderTexture(entry.RenderTexture);
            context.Targets.TryRemoveOffscreen(target, out _);
            return RenderResult.Success();
        }
        catch (Exception exception)
        {
            return RenderResult.Failure(
                RenderErrorCode.ResourceDestructionFailed,
                "Raylib could not destroy the offscreen render target.",
                exception.Message);
        }
    }

    public unsafe RenderResult<RenderImage> ReadRenderTarget(RenderTargetHandle target)
    {
        var targetResult = GetOffscreenTarget(target);
        if (!targetResult.IsSuccess)
        {
            return Failure<RenderImage>(targetResult.Error!);
        }

        Image image = default;
        Color* colors = null;

        try
        {
            image = NativeRaylib.LoadImageFromTexture(targetResult.Value.RenderTexture.Texture);
            if (!NativeRaylib.IsImageValid(image))
            {
                return RenderResult<RenderImage>.Failure(
                    RenderErrorCode.ReadbackFailed,
                    "Raylib could not read the offscreen render target.");
            }

            NativeRaylib.ImageFlipVertical(ref image);
            colors = NativeRaylib.LoadImageColors(image);
            if (colors is null)
            {
                return RenderResult<RenderImage>.Failure(
                    RenderErrorCode.ReadbackFailed,
                    "Raylib could not convert the readback image to RGBA pixels.");
            }

            var pixels = ImmutableArray.CreateBuilder<RenderColor>(checked(image.Width * image.Height));
            for (var index = 0; index < image.Width * image.Height; index++)
            {
                var color = colors[index];
                pixels.Add(new RenderColor(color.R, color.G, color.B, color.A));
            }

            return RenderResult<RenderImage>.Success(
                new RenderImage(image.Width, image.Height, pixels.MoveToImmutable()));
        }
        catch (Exception exception)
        {
            return RenderResult<RenderImage>.Failure(
                RenderErrorCode.ReadbackFailed,
                "Raylib could not read the offscreen render target.",
                exception.Message);
        }
        finally
        {
            if (colors is not null)
            {
                NativeRaylib.UnloadImageColors(colors);
            }

            if (NativeRaylib.IsImageValid(image))
            {
                NativeRaylib.UnloadImage(image);
            }
        }
    }

    public RenderResult SaveRenderTargetPng(RenderTargetHandle target, string path)
    {
        var targetResult = GetOffscreenTarget(target);
        if (!targetResult.IsSuccess)
        {
            return Failure(targetResult.Error!);
        }

        if (string.IsNullOrWhiteSpace(path) ||
            !string.Equals(Path.GetExtension(path), ".png", StringComparison.OrdinalIgnoreCase))
        {
            return RenderResult.Failure(
                RenderErrorCode.ImageExportFailed,
                "The output path must use the .png extension.");
        }

        string fullPath;
        try
        {
            fullPath = Path.GetFullPath(path);
        }
        catch (Exception exception)
        {
            return RenderResult.Failure(
                RenderErrorCode.ImageExportFailed,
                "The PNG output path is invalid.",
                exception.Message);
        }

        var directory = Path.GetDirectoryName(fullPath);
        if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
        {
            return RenderResult.Failure(
                RenderErrorCode.ImageExportFailed,
                "The PNG output directory does not exist.");
        }

        Image image = default;
        try
        {
            image = NativeRaylib.LoadImageFromTexture(targetResult.Value.RenderTexture.Texture);
            if (!NativeRaylib.IsImageValid(image))
            {
                return RenderResult.Failure(
                    RenderErrorCode.ImageExportFailed,
                    "Raylib could not read the offscreen target for PNG export.");
            }

            NativeRaylib.ImageFlipVertical(ref image);
            return NativeRaylib.ExportImage(image, fullPath)
                ? RenderResult.Success()
                : RenderResult.Failure(
                    RenderErrorCode.ImageExportFailed,
                    "Raylib failed to export the offscreen target as PNG.");
        }
        catch (Exception exception)
        {
            return RenderResult.Failure(
                RenderErrorCode.ImageExportFailed,
                "Raylib failed to export the offscreen target as PNG.",
                exception.Message);
        }
        finally
        {
            if (NativeRaylib.IsImageValid(image))
            {
                NativeRaylib.UnloadImage(image);
            }
        }
    }

    public RenderResult Shutdown()
    {
        if (context.RendererShutdown)
        {
            return RenderResult.Success();
        }

        if (Environment.CurrentManagedThreadId != context.PlatformThreadId)
        {
            return RenderResult.Failure(
                RenderErrorCode.WrongThread,
                "Raylib rendering must shut down on the platform thread.");
        }

        if (context.WindowClosed)
        {
            return RenderResult.Failure(
                RenderErrorCode.InvalidState,
                "The Raylib window closed before renderer resources were released.");
        }

        string? failure = null;
        foreach (var entry in context.Targets.RemoveAllOffscreen())
        {
            try
            {
                NativeRaylib.UnloadRenderTexture(entry.RenderTexture);
            }
            catch (Exception exception)
            {
                failure ??= exception.Message;
            }
        }

        context.RendererShutdown = true;
        return failure is null
            ? RenderResult.Success()
            : RenderResult.Failure(
                RenderErrorCode.ResourceDestructionFailed,
                "One or more Raylib render targets could not be released.",
                failure);
    }

    private RenderResult<RaylibRenderTargetEntry> GetOffscreenTarget(RenderTargetHandle target)
    {
        var access = context.CheckRenderAccess();
        if (!access.IsSuccess)
        {
            return Failure<RaylibRenderTargetEntry>(access.Error!);
        }

        if (!context.Targets.TryGet(target, out var entry))
        {
            return RenderResult<RaylibRenderTargetEntry>.Failure(
                RenderErrorCode.InvalidHandle,
                "The render target handle is invalid.");
        }

        return entry.Kind == RaylibRenderTargetKind.Window
            ? RenderResult<RaylibRenderTargetEntry>.Failure(
                RenderErrorCode.UnsupportedTargetOperation,
                "Readback and image export require an offscreen render target.")
            : RenderResult<RaylibRenderTargetEntry>.Success(entry);
    }

    private static RenderResult<T> Failure<T>(RenderError error) =>
        RenderResult<T>.Failure(error.Code, error.Message, error.Detail);

    private static RenderResult Failure(RenderError error) =>
        RenderResult.Failure(error.Code, error.Message, error.Detail);

    private static void Execute(RenderFrame frame)
    {
        foreach (var command in frame.Commands)
        {
            switch (command)
            {
                case ClearTargetCommand clear:
                    NativeRaylib.ClearBackground(ToNative(clear.Color));
                    break;
                case DrawRectangleCommand rectangle:
                    var bounds = rectangle.Bounds;
                    NativeRaylib.DrawRectangleRec(
                        new NativeRectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height),
                        ToNative(rectangle.Color));
                    break;
                default:
                    throw new InvalidOperationException(
                        $"Unsupported render command: {command.GetType().FullName}");
            }
        }
    }

    private static Color ToNative(RenderColor color) =>
        new(color.R, color.G, color.B, color.A);
}
