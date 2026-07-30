using Principle.Rendering.Abstractions;
using NativeRaylib = Raylib_cs.Raylib;

namespace Principle.Rendering.Raylib;

internal sealed class RaylibWindowOutput(RaylibContext context) : IWindowOutput
{
    public RenderTargetHandle Target => context.WindowTarget;

    public RenderResult Resize(RenderSurfaceSize size)
    {
        var access = context.CheckRenderAccess();
        if (!access.IsSuccess)
        {
            return access;
        }

        if (size.LogicalWidth <= 0 ||
            size.LogicalHeight <= 0 ||
            size.DrawableWidth <= 0 ||
            size.DrawableHeight <= 0)
        {
            return RenderResult.Failure(
                RenderErrorCode.InvalidState,
                "Window output dimensions must be positive.");
        }

        context.WindowSize = size;
        return RenderResult.Success();
    }

    public RenderResult Present()
    {
        var access = context.CheckRenderAccess();
        if (!access.IsSuccess)
        {
            return access;
        }

        try
        {
            NativeRaylib.SwapScreenBuffer();
            return RenderResult.Success();
        }
        catch (Exception exception)
        {
            return RenderResult.Failure(
                RenderErrorCode.BackendFailure,
                "Raylib failed while presenting the window.",
                exception.Message);
        }
    }
}
