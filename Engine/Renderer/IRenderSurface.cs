namespace Principle.API.Renderer;

public interface IRenderSurface : IDisposable
{
    string SurfaceTitle { get; set; }
    bool IsEnding { get; }

    int Width { get; }
    int Height { get; }
    int XPos { get; }
    int YPos { get; }

    event Action<(int, int)>? Resized;
    event Action<(int, int)>? Moved;
    event Action? Closing;

    event Action? Load;
    event Action<double>? Update;
    event Action<double>? Render;

    void Begin();
    void End();
}