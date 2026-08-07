namespace Principle.Engine;

internal sealed class ApplicationLifetime : IApplicationLifetime
{
    public bool IsRunning => _isRunning;
    public bool ShutdownRequested => _shutdownRequested;

    private volatile bool _shutdownRequested = false;
    private volatile bool _isRunning = false;
    
    public void RequestShutdown()
    {
        _shutdownRequested = true;
    }

    public void MarkRunning()
    {
        _isRunning = true;
    }

    public void MarkStopped()
    {
        _isRunning = false;
    }
}