namespace Principle.Engine;

public sealed class EngineHost
{
    private readonly ApplicationLifetime _lifetime = new();

    public void RequestShutdown() => _lifetime.RequestShutdown();

    public void Run(PrincipleGame game)
    {
        Application.Attach(_lifetime);

        try
        {
            game.PreInitialize();
            game.Initialize();
            game.PostInitialize();

            _lifetime.MarkRunning();

            game.TickScheduler.Start();

            while (!_lifetime.ShutdownRequested)
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
