using Principle.Engine.Simulation;
using Serilog;
using Serilog.Events;

namespace Principle.Engine;

public sealed class Application
{
    private readonly TickScheduler _tickScheduler;
    private bool _requestedShutdown;

    public static Application CreateNew()
    {
        var application = new Application();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "PrincipleEngine")
            .WriteTo.Debug()
            .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
            .WriteTo.File("logs/engine-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        return application;
    }

    public void Start()
    {
        try
        {
            Log.Information("Starting engine application");

            _tickScheduler.Start();

            while (!_requestedShutdown)
            {
                _tickScheduler.Tick();

                Thread.Sleep(1);
            }
        }
        catch (Exception e)
        {
            Log.Fatal(e, "An unknown error occurred when attempting to shutdown the engine!");
        }
        finally
        {
            Log.Information("Ending engine application");
            Log.CloseAndFlush();
        }

    }

    public void RequestShutdown()
    {
        Log.Debug("Shutdown of engine application requested");
        _requestedShutdown = true;
    }

    private Application()
    {
        _tickScheduler = new TickScheduler();
    }
}
