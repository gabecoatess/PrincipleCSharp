using System.Reflection;
using Principle.Contracts;
using Principle.Engine;
using TestGameProject;

namespace Principle.Runtime;

public static class Program
{
    private static bool OpenWindow = true;
    private static TickScheduler Scheduler = new TickScheduler();

    public static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            ParseArgs(args);
        }

        if (OpenWindow)
        {
            throw new NotImplementedException("Window mode is not yet implemented!");
        }
        
        MyGame application = new MyGame();
        application.Test();
    }

    private static void ParseArgs(string[] args)
    {
        foreach (string arg in args)
        {
            Console.WriteLine($"Used arg: '{arg}'");

            switch (arg)
            {
                case "-nw":
                case "--no-window":
                    OpenWindow = false;
                    
                    break;
                
                default:
                    break;
            }
        }
    }
}