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
        _world.Create(new Health(100.0f), new Hunger(100.0f), new PlayerTag());

        for (var i = 0; i < 3; i++)
        {
            _world.Create(new Health(50.0f), new ZombieTag());
        }

        TickScheduler.AddTickSchedule(nameof(HungerDepletion), new HungerDepletion(_world), 2.0);
    }

    protected override void OnShutdown() => _world.Dispose();
}
