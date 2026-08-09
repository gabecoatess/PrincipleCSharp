using System.Diagnostics;
using Arch.Core;
using Principle.ECS;
using TestGameProject.Components;
using TestGameProject.Tags;

namespace TestGameProject.Systems;

internal class EatPlayerSystem(World world) : IPrincipleSystem
{
    private static readonly QueryDescription Query = new QueryDescription().WithAll<HungerComponent, ZombieTag>();
    private static readonly QueryDescription PlayerQuery = new QueryDescription().WithAll<HealthComponent, PlayerTag>();

    public void Tick()
    {
        world.Query(in Query, (ref HungerComponent hungerComponent) =>
        {
            if (hungerComponent.Value < 50.0f)
            {
                world.Query(in PlayerQuery, (ref HealthComponent healthComponent) =>
                {
                    if (healthComponent.Value <= 0.0f)
                    {
                        return;
                    }

                    healthComponent.Value -= 10.0f;
                    Debug.WriteLine("Zombie ate 10 health from player!");
                });
            }
        });
    }
}
