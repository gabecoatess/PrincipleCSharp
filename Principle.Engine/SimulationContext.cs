using Arch.Buffer;
using Arch.Core;

namespace Principle.Engine;

public sealed class SimulationContext(World world) : IDisposable
{
    public World World { get; } = world ?? throw new ArgumentNullException(nameof(world));
    public CommandBuffer Commands { get; } = new();

    public void Commit() => Commands.Playback(World);

    public void Dispose()
    {
        Commands.Dispose();
        World.Dispose();
    }
}
