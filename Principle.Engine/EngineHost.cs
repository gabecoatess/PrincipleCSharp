namespace Principle.Engine;

public sealed class EngineHost
{
    
    public void Run(PrincipleGame game)
    {
        ApplicationLifetime lifetime = new ApplicationLifetime();
        Application.Attach(lifetime);

        Thread? simulationThread = null;
        
        try
        {
            game.PreInitialize();
            game.Initialize();
            game.PostInitialize();
        }

        lifetime.MarkRunning();

        simulationThread = new Thread(() => SimulationLoop(lifetime, game));

        
    }

    private void SimulationLoop(ApplicationLifetime lifetime, PrincipleGame game)
    {
        try
        {
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

            simulationThread?.Join();

            lifetime.MarkStopped();
            Application.Detach(lifetime);
        }
    }
}