using System.Diagnostics;
using Arch.Buffer;
using Arch.Core;
using Principle.ECS;
using TestGameProject.Components;
using TestGameProject.Tags;

namespace TestGameProject.Systems;

internal class HealthSystem(World world, CommandBuffer commands) : IPrincipleSystem
{
    private static readonly QueryDescription Query = new QueryDescription().WithAll<HealthComponent>();

    public void Tick()
    {
        world.Query(in Query, (ref Entity entity, ref HealthComponent healthComponent) =>
        {
            if (healthComponent.Value > 0.0f)
            {
                return;
            }

            Debug.WriteLine("Entity is dead!");
            commands.Add<DeadTag>(entity);
            commands.Remove<HealthComponent>(entity);
        });
    }
}
