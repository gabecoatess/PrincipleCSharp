using EngineAPI;

namespace PrincipleRuntime;

public class Program
{
    static void Main(string[] args)
    {
        Application myApp = new Application();
        myApp.Initialize();
        myApp.Run();
    }
}