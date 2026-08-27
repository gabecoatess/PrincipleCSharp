using Serilog;
using Serilog.Events;

namespace Principle.Engine.Logging;

internal static class LogCore
{
    private static readonly Lazy<ILogger> LazyLogger = new(InitializeSerilog);

    public static ILogger Instance => LazyLogger.Value;

    private static ILogger InitializeSerilog()
    {
        // TODO: Allow minimum level to be configured at runtime (verbose is currently impossible to use without recomp)
        return new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "PrincipleEngine")
            .Enrich.WithThreadId()
            .WriteTo.Debug(outputTemplate: "[{LogSource}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {LogSource}] [{Level:u3}] ({Application}) {Message:lj}{NewLine}{Exception}", restrictedToMinimumLevel: LogEventLevel.Information)
            .WriteTo.File(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{LogSource}] [{Level:u3}] [Thread {ThreadId}] ({Application}) {Message:lj}{NewLine}{Exception}", path: "logs/engine-.log", rollingInterval: RollingInterval.Day)
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
