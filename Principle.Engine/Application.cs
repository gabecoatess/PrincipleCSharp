using Principle.Engine.Simulation;

namespace Principle.Engine;

public sealed class Application
{
    private readonly TickScheduler _tickScheduler;
    private bool _requestedShutdown;

    public static Application CreateNew()
    {
        var application = new Application();

        return application;
    }

    public void Start()
    {
        _tickScheduler.Start();

        while (!_requestedShutdown)
        {
            _tickScheduler.Tick();

            Thread.Sleep(1);
        }
    }

    public void RequestShutdown()
    {
        _requestedShutdown = true;
    }

    private Application()
    {
        _tickScheduler = new TickScheduler();
    }
}
