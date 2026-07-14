using Principle.API.Renderer;
using Principle.Renderer;

namespace Principle.Runtime;

public sealed class Application : IApplication
{
    private IRenderer? _primaryRenderer = null;
    private IRenderSurface? _primarySurface = null;

    public void Initialize()
    {
        Console.WriteLine("Initializing application");
    }

    public void Run()
    {
        Console.WriteLine("Running application");

        while (_primarySurface?.IsEnding == false)
        {
            _primarySurface.PollEvents();
        }
    }

    public void BuildSurface(RenderSurfaceOptions options)
    {
        _primarySurface = new VeldridRenderSurface(options);
    }

    private void HandleLoad()
    {
    }

    private void HandleRender(double dt)
    {
        _primaryRenderer?.ClearColor();
        _primaryRenderer?.Render();
    }
}