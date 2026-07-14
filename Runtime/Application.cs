using Principle.API.Renderer;
using Principle.Renderer;
using System.Drawing;

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

        if (_primaryRenderer == null)
        {
            Console.WriteLine("[WARNING] No renderer was provided!");
        }

        if (_primarySurface == null)
        {
            Console.WriteLine("[WARNING] No render surface was provided!");
        }

        while (_primarySurface?.IsEnding == false)
        {
            _primarySurface.PollEvents();

            if (_primarySurface.IsEnding == false)
            {
                _primaryRenderer?.PrepareFrame();

                _primaryRenderer?.ClearColor();
                _primaryRenderer?.Render();

                _primaryRenderer?.FinalizeFrame();
            }
        }

        if (_primaryRenderer != null)
        {
            _primaryRenderer.Dispose();
        }

        Console.WriteLine("Quitting application");
    }

    public IRenderSurface BuildSurface(RenderSurfaceOptions options)
    {
        _primarySurface = new VeldridRenderSurface(options);
        return _primarySurface;
    }

    public IRenderer BuildRenderer(IRenderSurface renderSurface)
    {
        _primaryRenderer = new VeldridRenderer();
        _primaryRenderer.Initialize(renderSurface);
        return _primaryRenderer;
    }
}