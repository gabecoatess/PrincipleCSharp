namespace Principle.Engine;

internal interface IApplicationLifetime
{
    public bool IsRunning { get; }
    public bool ShutdownRequested { get; }

    public void RequestShutdown();
}
