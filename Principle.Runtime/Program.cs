using System.Diagnostics;

namespace Principle.Runtime;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            ParseArgs(args);
        }
    }

    private static void ParseArgs(string[] args)
    {
        foreach (var arg in args)
        {
            Debug.WriteLine($"[Runtime] Used arg: '{arg}'");
        }
    }
}
