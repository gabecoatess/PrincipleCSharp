using Principle.Platform;
using Principle.Rendering.Abstractions;

namespace Principle.Rendering;

public sealed record ShutdownReport(RenderError? RendererError, PlatformError? WindowError)
{
    public bool IsSuccess => RendererError is null && WindowError is null;
}

public sealed class RenderSession : IDisposable
{
    private ShutdownReport? _shutdownReport;

    public RenderSession(IRenderer renderer, IWindowOutput windowOutput, IPlatformWindow window)
    {
        Renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        WindowOutput = windowOutput ?? throw new ArgumentNullException(nameof(windowOutput));
        Window = window ?? throw new ArgumentNullException(nameof(window));
    }

    public IRenderer Renderer { get; }

    public IWindowOutput WindowOutput { get; }

    public IPlatformWindow Window { get; }

    public bool IsShutdown => _shutdownReport is not null;

    public RenderResult RenderToWindow(RenderFrame frame)
    {
        if (IsShutdown)
        {
            return RenderResult.Failure(RenderErrorCode.Disposed, "The render session is shut down.");
        }

        var submit = Renderer.Submit(WindowOutput.Target, frame);
        return submit.IsSuccess ? WindowOutput.Present() : submit;
    }

    public RenderResult ApplyWindowResize(WindowState state)
    {
        if (IsShutdown)
        {
            return RenderResult.Failure(RenderErrorCode.Disposed, "The render session is shut down.");
        }

        if (!state.WasResized)
        {
            return RenderResult.Success();
        }

        return WindowOutput.Resize(new RenderSurfaceSize(
            state.LogicalWidth,
            state.LogicalHeight,
            state.DrawableWidth,
            state.DrawableHeight));
    }

    public ShutdownReport Shutdown()
    {
        if (_shutdownReport is not null)
        {
            return _shutdownReport;
        }

        var rendererResult = Renderer.Shutdown();
        var windowResult = Window.Close();

        _shutdownReport = new ShutdownReport(rendererResult.Error, windowResult.Error);
        return _shutdownReport;
    }

    public void Dispose()
    {
        Shutdown();
    }
}
