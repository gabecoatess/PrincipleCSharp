using Principle.Engine.Logging;
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
        try
        {
            EngineLog.Information("Starting engine application");

            _tickScheduler.Start();

            while (!_requestedShutdown)
            {
                _tickScheduler.Tick();

                Thread.Sleep(1);
            }
        }
        catch (Exception e)
        {
            EngineLog.Fatal(e, "A fatal error occurred while the engine was running");
            throw;
        }
        finally
        {
            EngineLog.Information("Ending engine application");
            LogCore.Shutdown();
        }

    }

    public void RequestShutdown()
    {
        EngineLog.Debug("Shutdown of engine application requested");
        _requestedShutdown = true;
    }

    private Application()
    {
        _tickScheduler = new TickScheduler();
    }
}
