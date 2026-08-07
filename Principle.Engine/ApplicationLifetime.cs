namespace Principle.Engine;

internal sealed class ApplicationLifetime : IApplicationLifetime
{
    public bool IsRunning { get; private set; } = false;
    public bool ShutdownRequested { get; private set; } = false;
    
    public void RequestShutdown()
    {
        ShutdownRequested = true;
    }

    public void MarkRunning()
    {
        IsRunning = true;
    }

    public void MarkStopped()
    {
        IsRunning = false;
    }
}