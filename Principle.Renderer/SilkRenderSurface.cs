using Principle.API.Renderer;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Principle.Renderer;

public sealed class SilkRenderSurface : IRenderSurface
{
    private readonly IWindow _window;

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

    public int Width => _window.Size.X;
    public int Height => _window.Size.Y;
    public int XPos => _window.Position.X;
    public int YPos => _window.Position.Y;

    public bool IsEnding => _window.IsClosing;
    #endregion

    public SilkRenderSurface(RenderSurfaceOptions options)
    {
        _window = Window.Create(new WindowOptions
        {
            IsVisible = options.isVisible,
            Position = new Vector2D<int>(options.xPos, options.yPos),
            Size = new Vector2D<int>(options.width, options.height),
            FramesPerSecond = 0.0,
            UpdatesPerSecond = 0.0,
            API = GraphicsAPI.Default,
            Title = options.title,
            WindowState = ToWindowState(options.surfaceState),
            WindowBorder = ToWindowBorder(options.surfaceBorder),
            ShouldSwapAutomatically = true,
            VideoMode = VideoMode.Default
        });

        _window.Load += HandleLoad;
        _window.Update += HandleUpdate;
        _window.Render += HandleRender;
    }

    public GL CreateGraphicsContext()
    {
        return _window.CreateOpenGL();
    }

    public void Begin()
    {
        _window.Run();
    }

    public void End()
    {
        _window.Close();
    }

    public void Dispose()
    {
        _window.Dispose();
    }

    private void HandleLoad() => Load?.Invoke();
    private void HandleUpdate(double dt) => Update?.Invoke(dt);
    private void HandleRender(double dt) => Render?.Invoke(dt);

    private static SurfaceState ToSurfaceState(WindowState state) => state switch
    {
        WindowState.Normal => SurfaceState.Normal,
        WindowState.Minimized => SurfaceState.Minimized,
        WindowState.Maximized => SurfaceState.Maximized,
        WindowState.Fullscreen => SurfaceState.Fullscreen,
        _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
    };

    private static WindowState ToWindowState(SurfaceState state) => state switch
    {
        SurfaceState.Normal => WindowState.Normal,
        SurfaceState.Minimized => WindowState.Minimized,
        SurfaceState.Maximized => WindowState.Maximized,
        SurfaceState.Fullscreen => WindowState.Fullscreen,
        _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
    };

    private static SurfaceBorder ToSurfaceBorder(WindowBorder border) => border switch
    {
        WindowBorder.Resizable => SurfaceBorder.Resizable,
        WindowBorder.Fixed => SurfaceBorder.Fixed,
        WindowBorder.Hidden => SurfaceBorder.Hidden,
        _ => throw new ArgumentOutOfRangeException(nameof(border), border, null)
    };

    private static WindowBorder ToWindowBorder(SurfaceBorder border) => border switch
    {
        SurfaceBorder.Resizable => WindowBorder.Resizable,
        SurfaceBorder.Fixed => WindowBorder.Fixed,
        SurfaceBorder.Hidden => WindowBorder.Hidden,
        _ => throw new ArgumentOutOfRangeException(nameof(border), border, null)
    };
}