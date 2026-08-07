using Principle.Contracts;

namespace Principle.Engine;

public sealed class EngineHost
{
    private ApplicationLifetime? _lifetime = null;
    
    public void Run(PrincipleGame game)
    {
        _lifetime = new ApplicationLifetime();
        Application.Attach(_lifetime);
        
        try
        {
            game.PreInitialize();
            game.Initialize();
            game.PostInitialize();
            
            _lifetime.MarkRunning();

            while (!_lifetime.ShutdownRequested && _lifetime.IsRunning)
            {
                game.TickScheduler.Tick();
            }
        }
        finally
        {
            try
            {
                game.Shutdown();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
            
            _lifetime.MarkStopped();
            Application.Detach(_lifetime);
        }
    }
}