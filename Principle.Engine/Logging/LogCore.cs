using Serilog;
using Serilog.Events;

namespace Principle.Engine.Logging;

internal static class LogCore
{
    private static readonly Lazy<ILogger> LazyLogger = new(InitializeSerilog);

    public static ILogger Instance => LazyLogger.Value;

    private static ILogger InitializeSerilog()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "PrincipleEngine")
            .WriteTo.Debug()
            .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
            .WriteTo.File("logs/engine-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }

    public static void Shutdown()
    {
        if (!LazyLogger.IsValueCreated)
        {
            return;
        }

        if (LazyLogger.Value is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
