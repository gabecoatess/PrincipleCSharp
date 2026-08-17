using Arch.Core;
using Principle.Engine;
using TestGameProject.Components;
using TestGameProject.Systems;
using TestGameProject.Tags;

namespace TestGameProject;

public sealed class MyGame : PrincipleGame
{
    private readonly World _world;

    public MyGame() : this(World.Create())
    {
    }

    private MyGame(World world) : base(world)
    {
        _world = world;
    }

    protected override void OnInitialize()
    {
        _world.Create(
            new HealthComponent(100.0f),
            new PlayerTag()
        );

        for (var i = 0; i < 3; i++)
        {
            _world.Create(
                new HealthComponent(50.0f),
                new HungerComponent(100.0f),
                new ZombieTag()
            );
        }

        var twoHz = TickScheduler.AddTickSchedule("TwoHz", 2.0);
        var oneHz = TickScheduler.AddTickSchedule("OneHz", 1.0);
        var thirtyHz = TickScheduler.AddTickSchedule("ThirtyHz", 30.0);

        twoHz.AddSystem(new HungerDepletionSystem(_world));
        oneHz.AddSystem(new EatPlayerSystem(_world));
        thirtyHz.AddSystem(new HealthSystem(_world, Context.Commands));
    }
}
