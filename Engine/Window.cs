using Silk.NET.Input;
using Silk.NET.Windowing;
using System.Drawing;

namespace EngineAPI;

public class Window
{
    private IWindow _activeWindow;
    private WindowInputContext _inputContext;
    private Renderer _renderer;

    public Window(string title, int width, int height)
    {
        WindowOptions options = new WindowOptions
        {
            IsVisible = true,
            Position = new Silk.NET.Maths.Vector2D<int>(50, 50),
            Size = new Silk.NET.Maths.Vector2D<int>(width, height),
            FramesPerSecond = 0.0,
            UpdatesPerSecond = 0.0,
            API = GraphicsAPI.Default,
            Title = title,
            WindowState = WindowState.Normal,
            WindowBorder = WindowBorder.Resizable,
            ShouldSwapAutomatically = true,
            VideoMode = VideoMode.Default
        };

        _activeWindow = Silk.NET.Windowing.Window.Create(options);

        _activeWindow.Load += HandleLoad;
        _activeWindow.Update += HandleUpdate;
        _activeWindow.Render += HandleRender;

        _activeWindow.Run();
    }

    private void HandleLoad()
    {
        _inputContext = new WindowInputContext(_activeWindow.CreateInput());
        _inputContext.RegisterInputs();

        _inputContext.KeyPressed += HandleKeyPressed;

        _renderer = new Renderer(_activeWindow);
        _renderer.SetClearColor(Color.Red);
    }

    private void HandleUpdate(double dt)
    {

    }

    private void HandleRender(double dt)
    {
        _renderer.ClearColorBufferBit();
        _renderer.Render();
    }

    private void HandleKeyPressed(object? sender, Key key)
    {
        if (key == Key.Escape)
        {
            _activeWindow.Close();
        }
    }
}