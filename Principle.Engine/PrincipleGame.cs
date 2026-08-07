using System.Runtime.CompilerServices;
using Principle.Contracts;

[assembly: InternalsVisibleTo("Principle.Runtime")]

namespace Principle.Engine;

public abstract class PrincipleGame : IPrincipleGame
{
    public ITickScheduler TickScheduler { get; } = new TickScheduler();
    public bool ShutdownRequested { get; private set; } = false;
    
    public virtual void PreInitialize()
    {
        Console.WriteLine("[Engine] PreInitialize");
    }

    public virtual void Initialize()
    {
        Console.WriteLine("[Engine] Initialize");
    }

    public virtual void PostInitialize()
    {
        Console.WriteLine("[Engine] PostInitialize");
    }

    public virtual void PreShutdown()
    {
        Console.WriteLine("[Engine] PreShutdown");
    }

    public virtual void Shutdown()
    {
        Console.WriteLine("[Engine] Shutdown");
    }
}