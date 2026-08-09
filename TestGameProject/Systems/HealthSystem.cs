using System.Diagnostics;
using Arch.Core;
using Principle.ECS;
using TestGameProject.Components;

namespace TestGameProject.Systems;

internal class HealthSystem(World world) : IPrincipleSystem
{
    private static readonly QueryDescription Query = new QueryDescription().WithAll<HealthComponent, IsDeadComponent>();

    public void Tick()
    {
        world.Query(in Query, (ref HealthComponent healthComponent, ref IsDeadComponent isDeadComponent) =>
        {
            if (!(healthComponent.Value <= 0.0f) || isDeadComponent.Value)
            {
                return;
            }

            Debug.WriteLine("Entity is dead!");
            isDeadComponent.Value = true;
        });
    }
}
