using Principle.API.Renderer;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Principle.Renderer;

public class VeldridRenderSurface : IRenderSurface
{
    private readonly Sdl2Window _window;
    
    #region Events
    public event Action<(int, int)>? Resized;
    public event Action<(int, int)>? Moved;
    public event Action? Closing;
    
    public event Action? Load;
    public event Action<double>? Update;
    public event Action<double>? Render;
    #endregion
    
    #region Properties

    public string SurfaceTitle
    {
        get => _window.Title;
        set => _window.Title = value;
    }

    public int Width => _window.Width;
    public int Height => _window.Height;
    public int XPos => _window.X;
    public int YPos => _window.Y;
    public bool IsEnding => !_window.Exists;
    #endregion

    public VeldridRenderSurface(RenderSurfaceOptions options)
    {
        _window = VeldridStartup.CreateWindow(new WindowCreateInfo
        {
            X = options.xPos,
            Y = options.yPos,
            WindowWidth = options.width,
            WindowHeight = options.height,
            WindowTitle = options.title,
        });
    }
    
    public void Dispose()
    {
    }
    
    public void Begin()
    {
    }

    public void End()
    {
        _window.Close();
    }
}