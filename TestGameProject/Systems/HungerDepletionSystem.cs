using System.Diagnostics;
using Arch.Core;
using Principle.Contracts;
using TestGameProject.Components;

namespace TestGameProject.Systems;

internal class HungerDepletionSystem(World world) : ITickSchedule
{
    private static readonly QueryDescription Query = new QueryDescription().WithAll<HungerComponent>();

    public void Tick()
    {
        world.Query(in Query, (Entity entity, ref HungerComponent hungerComponent) =>
        {
            if (hungerComponent.Value > 0.0f)
            {
                hungerComponent.Value -= 5f;

                if (hungerComponent.Value >= 50.0f)
                {
                    Debug.WriteLine($"HungerComponent depleted for entity {entity.Id}: {hungerComponent.Value}");
                }
            }
        });
    }
}
