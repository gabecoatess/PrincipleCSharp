using Principle.Platform;
using Principle.Rendering.Abstractions;

namespace Principle.Rendering.Tests;

public sealed class RenderSessionTests
{
    [Fact]
    public void RenderToWindowSubmitsBeforePresenting()
    {
        var calls = new List<string>();
        var renderer = new FakeRenderer(calls);
        var output = new FakeWindowOutput(calls);
        var window = new FakeWindow(calls);
        var session = new RenderSession(renderer, output, window);

        var result = session.RenderToWindow(RenderFrame.Create());

        Assert.True(result.IsSuccess);
        Assert.Equal(["submit", "present"], calls);
    }

    [Fact]
    public void RenderToWindowSkipsPresentAfterSubmissionFailure()
    {
        var calls = new List<string>();
        var renderer = new FakeRenderer(calls)
        {
            SubmitResult = RenderResult.Failure(RenderErrorCode.InvalidFrame, "bad frame")
        };
        var session = new RenderSession(
            renderer,
            new FakeWindowOutput(calls),
            new FakeWindow(calls));

        var result = session.RenderToWindow(RenderFrame.Create());

        Assert.False(result.IsSuccess);
        Assert.Equal(["submit"], calls);
    }

    [Fact]
    public void ResizeIsForwardedOnlyWhenReported()
    {
        var calls = new List<string>();
        var output = new FakeWindowOutput(calls);
        var session = new RenderSession(
            new FakeRenderer(calls),
            output,
            new FakeWindow(calls));

        var unchanged = session.ApplyWindowResize(new WindowState(10, 20, 20, 40, false, false));
        var changed = session.ApplyWindowResize(new WindowState(30, 40, 60, 80, true, false));

        Assert.True(unchanged.IsSuccess);
        Assert.True(changed.IsSuccess);
        Assert.Equal(["resize"], calls);
        Assert.Equal(new RenderSurfaceSize(30, 40, 60, 80), output.LastSize);
    }

    [Fact]
    public void ShutdownReleasesRendererBeforeWindowAndIsIdempotent()
    {
        var calls = new List<string>();
        var session = new RenderSession(
            new FakeRenderer(calls),
            new FakeWindowOutput(calls),
            new FakeWindow(calls));

        var first = session.Shutdown();
        var second = session.Shutdown();

        Assert.True(first.IsSuccess);
        Assert.Same(first, second);
        Assert.Equal(["renderer-shutdown", "window-close"], calls);
    }

    [Fact]
    public void ShutdownPreservesRendererAndWindowFailures()
    {
        var calls = new List<string>();
        var renderer = new FakeRenderer(calls)
        {
            ShutdownResult = RenderResult.Failure(RenderErrorCode.BackendFailure, "renderer")
        };
        var window = new FakeWindow(calls)
        {
            CloseResult = PlatformResult.Failure(PlatformErrorCode.NativeFailure, "window")
        };
        var session = new RenderSession(renderer, new FakeWindowOutput(calls), window);

        var report = session.Shutdown();

        Assert.False(report.IsSuccess);
        Assert.Equal("renderer", report.RendererError!.Message);
        Assert.Equal("window", report.WindowError!.Message);
        Assert.Equal(["renderer-shutdown", "window-close"], calls);
    }

    private sealed class FakeRenderer(List<string> calls) : IRenderer
    {
        public RenderResult SubmitResult { get; init; } = RenderResult.Success();
        public RenderResult ShutdownResult { get; init; } = RenderResult.Success();

        public RenderResult Submit(RenderTargetHandle target, RenderFrame frame)
        {
            calls.Add("submit");
            return SubmitResult;
        }

        public RenderResult<RenderTargetHandle> CreateOffscreenTarget(RenderTargetDescription description) =>
            RenderResult<RenderTargetHandle>.Failure(RenderErrorCode.BackendFailure, "unused");

        public RenderResult DestroyRenderTarget(RenderTargetHandle target) =>
            RenderResult.Failure(RenderErrorCode.BackendFailure, "unused");

        public RenderResult<RenderImage> ReadRenderTarget(RenderTargetHandle target) =>
            RenderResult<RenderImage>.Failure(RenderErrorCode.BackendFailure, "unused");

        public RenderResult SaveRenderTargetPng(RenderTargetHandle target, string path) =>
            RenderResult.Failure(RenderErrorCode.BackendFailure, "unused");

        public RenderResult Shutdown()
        {
            calls.Add("renderer-shutdown");
            return ShutdownResult;
        }
    }

    private sealed class FakeWindowOutput(List<string> calls) : IWindowOutput
    {
        public RenderTargetHandle Target { get; } = new(7);
        public RenderSurfaceSize? LastSize { get; private set; }

        public RenderResult Resize(RenderSurfaceSize size)
        {
            calls.Add("resize");
            LastSize = size;
            return RenderResult.Success();
        }

        public RenderResult Present()
        {
            calls.Add("present");
            return RenderResult.Success();
        }
    }

    private sealed class FakeWindow(List<string> calls) : IPlatformWindow
    {
        public PlatformResult CloseResult { get; init; } = PlatformResult.Success();

        public PlatformResult<WindowState> PollEvents() =>
            PlatformResult<WindowState>.Success(default);

        public PlatformResult Close()
        {
            calls.Add("window-close");
            return CloseResult;
        }
    }
}
