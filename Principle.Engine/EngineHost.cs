namespace Principle.Engine;

public sealed class EngineHost
{
    public void Run(PrincipleGame game)
    {
        ApplicationLifetime lifetime = new ApplicationLifetime();
        Application.Attach(lifetime);
        
        try
        {
            game.PreInitialize();
            game.Initialize();
            game.PostInitialize();
            
            lifetime.MarkRunning();

            game.TickScheduler.Start();

            while (!lifetime.ShutdownRequested && lifetime.IsRunning)
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
            
            lifetime.MarkStopped();
            Application.Detach(lifetime);
        }
    }
}