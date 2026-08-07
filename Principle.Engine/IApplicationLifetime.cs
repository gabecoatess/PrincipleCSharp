namespace Principle.Engine;

internal interface IApplicationLifetime
{
    bool IsRunning { get; }
    bool ShutdownRequested { get; }

    void RequestShutdown();
}