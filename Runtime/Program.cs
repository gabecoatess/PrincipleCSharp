using Principle.API.Renderer;

namespace Principle.Runtime;

public class Program
{
    static void Main(string[] args)
    {
        Application myApp = new Application();
        myApp.Initialize();
        myApp.BuildSurface(RenderSurfaceOptions.Default);

        myApp.Run();
    }
}