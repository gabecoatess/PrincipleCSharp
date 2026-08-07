using Principle.Contracts;

namespace Principle.Engine;

public sealed class EngineHost
{
    private readonly ApplicationLifetime _lifetime = new ApplicationLifetime();
    
    public void Run(IPrincipleGame game)
    {
        Application.Attach(_lifetime);
        
        try
        {
            game.PreInitialize();
            game.Initialize();
            game.PostInitialize();
            
            _lifetime.MarkRunning();

            while (!_lifetime.ShutdownRequested)
            {
                game.TickScheduler.Tick();
            }
        }
        finally
        {
            game.PreShutdown();
            game.Shutdown();
            
            _lifetime.MarkStopped();
            Application.Detach(_lifetime);
        }
    }
}