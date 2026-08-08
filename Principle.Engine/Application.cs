namespace Principle.Engine;

public static class Application
{
    private static IApplicationLifetime? _lifetime;

    public static void Quit() => _lifetime?.RequestShutdown();

    internal static void Attach(IApplicationLifetime lifetime)
    {
        if (_lifetime is not null)
        {
            throw new InvalidOperationException("[Engine] Application already attached");
        }

        _lifetime = lifetime;
    }

    internal static void Detach(IApplicationLifetime lifetime)
    {
        if (ReferenceEquals(_lifetime, lifetime))
        {
            _lifetime = null;
        }
    }
}
