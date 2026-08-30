using Principle.Engine;
using Principle.Engine.Logging;

namespace Principle.Runtime;

internal static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            ParseArgs(args);
        }

        var application = Application.CreateNew();
        var applicationThread = new Thread(application.Start);

        applicationThread.Start();
        applicationThread.Join();
    }

    private static void ParseArgs(string[] args)
    {
        foreach (var arg in args)
        {
            EngineLog.Information($"[Runtime] Used arg: '{arg}'");
        }
    }
}
