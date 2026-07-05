namespace Principle.API.Renderer;

public interface IApplication
{
    void Initialize();
    void Run();
    void BuildSurface(RenderSurfaceOptions options);
}