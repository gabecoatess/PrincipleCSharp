using System.Diagnostics;
using Arch.Core;
using Principle.Contracts;
using TestGameProject.Components;

namespace TestGameProject.Systems;

internal class HungerDepletion(World world) : ITickSchedule
{
    private static readonly QueryDescription Query = new QueryDescription().WithAll<Hunger>();

    public void Tick()
    {
        world.Query(in Query, (Entity entity, ref Hunger hunger) =>
        {
            hunger.Value -= 0.5f;
            Debug.WriteLine($"Hunger depleted for entity {entity.Id}: {hunger.Value}");
        });
    }
}
