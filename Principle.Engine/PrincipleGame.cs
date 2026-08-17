using System.Diagnostics;
using Arch.Core;

namespace Principle.Engine;

public abstract class PrincipleGame
{
    protected SimulationContext Context { get; }
    public TickScheduler TickScheduler { get; }

    internal void PreInitialize() => OnPreInitialize();
    internal void Initialize() => OnInitialize();
    internal void PostInitialize() => OnPostInitialize();

    protected PrincipleGame(World world)
    {
        Context = new SimulationContext(world);
        TickScheduler = new TickScheduler(Context);
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
            Context.Dispose();
        }
    }

    protected virtual void OnPreInitialize() => Debug.WriteLine("[Engine] PreInitialize");
    protected virtual void OnInitialize() => Debug.WriteLine("[Engine] Initialize");
    protected virtual void OnPostInitialize() => Debug.WriteLine("[Engine] PostInitialize");
    protected virtual void OnPreShutdown() => Debug.WriteLine("[Engine] PreShutdown");
    protected virtual void OnShutdown() => Debug.WriteLine("[Engine] Shutdown");
}
