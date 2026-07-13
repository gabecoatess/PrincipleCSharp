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

        if (_primarySurface != null)
        {
            _primarySurface.Load += HandleLoad;
            _primarySurface.Render += HandleRender;
            _primarySurface.Begin();
        }
    }

    public void BuildSurface(RenderSurfaceOptions options)
    {
        _primarySurface = new VeldridRenderSurface(options);
    }

    private void HandleLoad()
    {
        if (_primarySurface is SilkRenderSurface silkSurface)
        {
            var glSurface = silkSurface!;
            _primaryRenderer = new GLRenderer(glSurface.CreateGraphicsContext());
            _primaryRenderer.Initialize();
        }
    }

    private void HandleRender(double dt)
    {
        _primaryRenderer?.ClearColor();
        _primaryRenderer?.Render();
    }
}