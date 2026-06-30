using EngineAPI;

namespace PrincipleRuntime;

public class Program
{
    static void Main(string[] args)
    {
        string appName = "PrincipleEngine";
        int x = 800;
        int y = 600;

        Application myApp = new Application();
        myApp.Initialize();
        myApp.Run();

        myApp.BuildWindow(appName, x, y);
    }
}