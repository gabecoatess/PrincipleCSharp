namespace Principle.API.Renderer;

public record RenderSurfaceOptions(
    string title, 
    int width, 
    int height,
    int xPos, 
    int yPos, 
    bool isVisible, 
    SurfaceBorder surfaceBorder, 
    SurfaceState surfaceState
    )
{
    public static RenderSurfaceOptions Default => new("Principle Engine", 800, 600, 100, 100, true, SurfaceBorder.Resizable, SurfaceState.Normal);
    public static RenderSurfaceOptions Empty => new("", 0, 0, 0, 0, false, SurfaceBorder.Hidden, SurfaceState.Normal);
}
