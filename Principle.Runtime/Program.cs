using System.Diagnostics;
using Principle.Engine;

namespace Principle.Runtime;

public static class Program
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
            Debug.WriteLine($"[Runtime] Used arg: '{arg}'");
        }
    }
}
