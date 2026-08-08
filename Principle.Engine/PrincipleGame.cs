using System.Diagnostics;

namespace Principle.Engine;

public abstract class PrincipleGame
{
    public TickScheduler TickScheduler { get; private set; } = new TickScheduler();

    internal void PreInitialize() => OnPreInitialize();
    internal void Initialize() => OnInitialize();
    internal void PostInitialize() => OnPostInitialize();

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

    protected virtual void OnPreInitialize() => Debug.WriteLine("[Engine] PreInitialize");
    protected virtual void OnInitialize() => Debug.WriteLine("[Engine] Initialize");
    protected virtual void OnPostInitialize() => Debug.WriteLine("[Engine] PostInitialize");
    protected virtual void OnPreShutdown() => Debug.WriteLine("[Engine] PreShutdown");
    protected virtual void OnShutdown() => Debug.WriteLine("[Engine] Shutdown");
}
