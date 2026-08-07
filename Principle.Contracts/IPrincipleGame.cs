namespace Principle.Contracts;

public interface IPrincipleGame
{
    ITickScheduler TickScheduler { get; }
    
    void PreInitialize();
    void Initialize();
    void PostInitialize();
    void PreShutdown();
    void Shutdown();
}