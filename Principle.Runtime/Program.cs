using System.Diagnostics;
using Principle.Engine;
using TestGameProject;

namespace Principle.Runtime;

public static class Program
{
    private static bool _openWindow = true;

    public static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            ParseArgs(args);
        }

        var host = new EngineHost();

        var engineHostThread = new Thread(() => host.Run(new MyGame()));
        engineHostThread.Start();

        if (_openWindow)
        {
            throw new NotImplementedException();
        }

        host.RequestShutdown();
        engineHostThread.Join();
    }

    private static void ParseArgs(string[] args)
    {
        foreach (var arg in args)
        {
            Debug.WriteLine($"[Runtime] Used arg: '{arg}'");

            _openWindow = arg switch
            {
                "-nw" or "--no-window" => false,
                _ => _openWindow
            };
        }
    }
}
