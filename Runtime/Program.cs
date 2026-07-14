using Principle.API.Renderer;

namespace Principle.Runtime;

public class Program
{
    static void Main(string[] args)
    {
        Application myApp = new Application();
        myApp.Initialize();
        
        var surface = myApp.BuildSurface(RenderSurfaceOptions.Default);
        var renderer = myApp.BuildRenderer(surface);

        myApp.Run();
    }
}