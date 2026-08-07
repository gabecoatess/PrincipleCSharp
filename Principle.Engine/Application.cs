namespace Principle.Engine;

public static class Application
{
    public static bool IsRunning => Lifetime?.IsRunning ?? false;
    
    private static IApplicationLifetime? Lifetime = null;

    public static void Quit()
    {
        Lifetime?.RequestShutdown();
    }

    internal static void Attach(IApplicationLifetime lifetime)
    {
        if (Lifetime is not null)
        {
            throw new InvalidOperationException("[Engine] Application already attached");
        }
        
        Lifetime = lifetime;
    }

    internal static void Detach(IApplicationLifetime lifetime)
    {
        if (ReferenceEquals(Lifetime, lifetime))
        {
            Lifetime = null;
        }
    }
}