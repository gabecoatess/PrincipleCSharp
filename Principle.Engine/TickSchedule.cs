using Principle.ECS;

namespace Principle.Engine;

public sealed class TickSchedule
{
    private readonly Dictionary<string, IPrincipleSystem> _systems = new();

    public void Tick()
    {
        foreach (var system in _systems.Values)
        {
            system.Tick();
        }
    }

    public void AddSystem(IPrincipleSystem principleSystem)
    {
        if (principleSystem == null)
        {
            throw new ArgumentNullException(nameof(principleSystem), "System cannot be null.");
        }

        if (!_systems.TryAdd(principleSystem.GetType().Name, principleSystem))
        {
            throw new InvalidOperationException("A system with the same name already exists.");
        }
    }

    public void AddSystem<T>(string name, T system, bool overwrite = false) where T : IPrincipleSystem
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("System name cannot be null or whitespace.", nameof(name));
        }

        if (!overwrite && _systems.ContainsKey(name))
        {
            throw new InvalidOperationException("A system with the same name already exists.");
        }

        _systems[name] = system;
    }

    public IPrincipleSystem? GetSystem(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("System name cannot be null or whitespace.", nameof(name));
        }

        return _systems.GetValueOrDefault(name);
    }
}
