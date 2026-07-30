using Principle.Platform;
using Principle.Rendering.Abstractions;
using Raylib_cs;
using NativeRaylib = Raylib_cs.Raylib;

namespace Principle.Rendering.Raylib;

public static class RaylibBackend
{
    private static int _activeContext;

    public static RenderResult<RenderSession> CreateSession(WindowDescription description)
    {
        if (string.IsNullOrWhiteSpace(description.Title) ||
            description.Width <= 0 ||
            description.Height <= 0)
        {
            return RenderResult<RenderSession>.Failure(
                RenderErrorCode.WindowCreationFailed,
                "A window requires a title and positive dimensions.");
        }

        if (Interlocked.CompareExchange(ref _activeContext, 1, 0) != 0)
        {
            return RenderResult<RenderSession>.Failure(
                RenderErrorCode.InvalidState,
                "Raylib supports only one active Principle render session.");
        }

        var windowCreated = false;
        try
        {
            var flags = default(ConfigFlags);
            if (description.IsResizable)
            {
                flags |= ConfigFlags.ResizableWindow;
            }

            if (!description.IsVisible)
            {
                flags |= ConfigFlags.HiddenWindow;
            }

            NativeRaylib.SetConfigFlags(flags);
            NativeRaylib.InitWindow(description.Width, description.Height, description.Title);
            windowCreated = NativeRaylib.IsWindowReady();

            if (!windowCreated)
            {
                Interlocked.Exchange(ref _activeContext, 0);
                return RenderResult<RenderSession>.Failure(
                    RenderErrorCode.WindowCreationFailed,
                    "Raylib did not create a usable window and graphics context.");
            }

            NativeRaylib.SetExitKey(KeyboardKey.Null);

            var targets = new RaylibRenderTargetRegistry();
            var windowTarget = targets.RegisterWindow();
            var context = new RaylibContext(
                targets,
                windowTarget,
                () => Interlocked.Exchange(ref _activeContext, 0))
            {
                WindowSize = new RenderSurfaceSize(
                    NativeRaylib.GetScreenWidth(),
                    NativeRaylib.GetScreenHeight(),
                    NativeRaylib.GetRenderWidth(),
                    NativeRaylib.GetRenderHeight())
            };

            var renderer = new RaylibRenderer(context);
            var output = new RaylibWindowOutput(context);
            var window = new RaylibPlatformWindow(context);

            return RenderResult<RenderSession>.Success(new RenderSession(renderer, output, window));
        }
        catch (Exception exception)
        {
            if (windowCreated)
            {
                try
                {
                    NativeRaylib.CloseWindow();
                }
                catch
                {
                    // Preserve the original creation error.
                }
            }

            Interlocked.Exchange(ref _activeContext, 0);
            return RenderResult<RenderSession>.Failure(
                RenderErrorCode.WindowCreationFailed,
                "Raylib failed while creating the render session.",
                exception.Message);
        }
    }
}
