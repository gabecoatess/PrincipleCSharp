namespace Principle.API.Renderer;

public interface IApplication
{
    void Initialize();
    void Run();
    IRenderSurface BuildSurface(RenderSurfaceOptions options);
    IRenderer BuildRenderer(IRenderSurface renderSurface);
}