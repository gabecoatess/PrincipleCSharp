namespace EngineAPI;

public class Application
{
    private Window? _primaryWindow = null;

    public void Initialize()
    {
        Console.WriteLine("Initializing application");
    }

    public void Run()
    {
        Console.WriteLine("Running application");
    }

    public void BuildWindow(string title, int defaultWidth, int defaultHeight)
    {
        _primaryWindow = new Window(title, defaultWidth, defaultHeight);
    }
}