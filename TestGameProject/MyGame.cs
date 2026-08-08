using Arch.Core;
using Principle.Engine;
using TestGameProject.Components;
using TestGameProject.Systems;
using TestGameProject.Tags;

namespace TestGameProject;

public class MyGame : PrincipleGame
{
    private readonly World _world = World.Create();

    protected override void OnInitialize()
    {
        _world.Create(
            new HealthComponent(100.0f),
            new IsDeadComponent(false),
            new PlayerTag()
        );

        for (var i = 0; i < 3; i++)
        {
            _world.Create(
                new HealthComponent(50.0f),
                new HungerComponent(100.0f),
                new IsDeadComponent(false),
                new ZombieTag()
            );
        }

        TickScheduler.AddTickSchedule(nameof(HungerDepletionSystem), new HungerDepletionSystem(_world), 2.0);
        TickScheduler.AddTickSchedule(nameof(EatPlayerSystem), new EatPlayerSystem(_world), 1.0);
        TickScheduler.AddTickSchedule(nameof(HealthSystem), new HealthSystem(_world), 30.0);
    }

    protected override void OnShutdown() => _world.Dispose();
}
