using Principle.Platform;
using Raylib_cs;
using NativeRaylib = Raylib_cs.Raylib;

namespace Principle.Rendering.Raylib;

internal sealed class RaylibPlatformWindow(RaylibContext context) : IPlatformWindow
{
    public PlatformResult<WindowState> PollEvents()
    {
        var access = context.CheckPlatformAccess();
        if (!access.IsSuccess)
        {
            return PlatformResult<WindowState>.Failure(
                access.Error!.Code,
                access.Error.Message,
                access.Error.Detail);
        }

        try
        {
            NativeRaylib.PollInputEvents();

            var state = new WindowState(
                NativeRaylib.GetScreenWidth(),
                NativeRaylib.GetScreenHeight(),
                NativeRaylib.GetRenderWidth(),
                NativeRaylib.GetRenderHeight(),
                NativeRaylib.IsWindowResized(),
                NativeRaylib.WindowShouldClose());

            context.WindowSize = new(
                state.LogicalWidth,
                state.LogicalHeight,
                state.DrawableWidth,
                state.DrawableHeight);

            return PlatformResult<WindowState>.Success(state);
        }
        catch (Exception exception)
        {
            return PlatformResult<WindowState>.Failure(
                PlatformErrorCode.NativeFailure,
                "Raylib failed while polling window events.",
                exception.Message);
        }
    }

    public PlatformResult Close()
    {
        if (context.WindowClosed)
        {
            return PlatformResult.Success();
        }

        if (Environment.CurrentManagedThreadId != context.PlatformThreadId)
        {
            return PlatformResult.Failure(
                PlatformErrorCode.WrongThread,
                "The Raylib window must close on the platform thread.");
        }

        try
        {
            NativeRaylib.CloseWindow();
            context.WindowClosed = true;
            context.ReleaseGlobalContext();
            return PlatformResult.Success();
        }
        catch (Exception exception)
        {
            return PlatformResult.Failure(
                PlatformErrorCode.NativeFailure,
                "Raylib failed while closing the window.",
                exception.Message);
        }
    }
}
