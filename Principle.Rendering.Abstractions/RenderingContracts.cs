namespace Principle.Rendering.Abstractions;

public interface IRenderer
{
    RenderResult Submit(RenderTargetHandle target, RenderFrame frame);

    RenderResult<RenderTargetHandle> CreateOffscreenTarget(RenderTargetDescription description);

    RenderResult DestroyRenderTarget(RenderTargetHandle target);

    RenderResult<RenderImage> ReadRenderTarget(RenderTargetHandle target);

    RenderResult SaveRenderTargetPng(RenderTargetHandle target, string path);

    RenderResult Shutdown();
}

public interface IWindowOutput
{
    RenderTargetHandle Target { get; }

    RenderResult Resize(RenderSurfaceSize size);

    RenderResult Present();
}
