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

        var twoHz = TickScheduler.AddTickSchedule("TwoHz", 2.0);
        var oneHz = TickScheduler.AddTickSchedule("OneHz", 1.0);
        var thirtyHz = TickScheduler.AddTickSchedule("ThirtyHz", 30.0);

        twoHz.AddSystem(new HungerDepletionSystem(_world));
        oneHz.AddSystem(new EatPlayerSystem(_world));
        thirtyHz.AddSystem(new HealthSystem(_world));
    }

    protected override void OnShutdown() => _world.Dispose();
}
