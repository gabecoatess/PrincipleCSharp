using Principle.Platform;
using Principle.Rendering.Abstractions;

namespace Principle.Rendering.Raylib;

internal sealed class RaylibContext
{
    private readonly Action _releaseGlobalContext;
    private bool _globalContextReleased;

    public RaylibContext(
        RaylibRenderTargetRegistry targets,
        RenderTargetHandle windowTarget,
        Action releaseGlobalContext)
    {
        Targets = targets;
        WindowTarget = windowTarget;
        _releaseGlobalContext = releaseGlobalContext;
        PlatformThreadId = Environment.CurrentManagedThreadId;
    }

    public int PlatformThreadId { get; }

    public RaylibRenderTargetRegistry Targets { get; }

    public RenderTargetHandle WindowTarget { get; }

    public bool RendererShutdown { get; set; }

    public bool WindowClosed { get; set; }

    public RenderSurfaceSize WindowSize { get; set; }

    public RenderResult CheckRenderAccess()
    {
        if (Environment.CurrentManagedThreadId != PlatformThreadId)
        {
            return RenderResult.Failure(
                RenderErrorCode.WrongThread,
                "Raylib rendering must execute on the platform thread.");
        }

        if (RendererShutdown)
        {
            return RenderResult.Failure(RenderErrorCode.Disposed, "The Raylib renderer is shut down.");
        }

        if (WindowClosed)
        {
            return RenderResult.Failure(
                RenderErrorCode.InvalidState,
                "The Raylib graphics context is closed.");
        }

        return RenderResult.Success();
    }

    public PlatformResult CheckPlatformAccess()
    {
        if (Environment.CurrentManagedThreadId != PlatformThreadId)
        {
            return PlatformResult.Failure(
                PlatformErrorCode.WrongThread,
                "Raylib platform calls must execute on the platform thread.");
        }

        if (WindowClosed)
        {
            return PlatformResult.Failure(PlatformErrorCode.Disposed, "The Raylib window is closed.");
        }

        return PlatformResult.Success();
    }

    public void ReleaseGlobalContext()
    {
        if (_globalContextReleased)
        {
            return;
        }

        _globalContextReleased = true;
        _releaseGlobalContext();
    }
}
