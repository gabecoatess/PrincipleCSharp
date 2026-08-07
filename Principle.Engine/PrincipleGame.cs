using Principle.Contracts;

namespace Principle.Engine;

public abstract class PrincipleGame
{
    public ITickScheduler TickScheduler { get; } = new TickScheduler();

    internal void PreInitialize()
    {
        OnPreInitialize();
    }

    internal void Initialize()
    {
        OnInitialize();
    }

    internal void PostInitialize()
    {
        OnPostInitialize();
    }

    internal void Shutdown()
    {
        try
        {
            OnPreShutdown();
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
        }
        finally
        {
            OnShutdown();
        }
    }
    
    protected virtual void OnPreInitialize()
    {
        Console.WriteLine("[Engine] PreInitialize");
    }

    protected virtual void OnInitialize()
    {
        Console.WriteLine("[Engine] Initialize");
    }

    protected virtual void OnPostInitialize()
    {
        Console.WriteLine("[Engine] PostInitialize");
    }

    protected virtual void OnPreShutdown()
    {
        Console.WriteLine("[Engine] PreShutdown");
    }

    protected virtual void OnShutdown()
    {
        Console.WriteLine("[Engine] Shutdown");
    }
}