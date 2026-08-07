using Principle.Engine;

namespace TestGameProject;

public class MyGame : PrincipleGame
{
    public override void Initialize()
    {
        base.Initialize();
        TickScheduler.AddTickSchedule("MyTickSchedule", new MyTickSchedule());
    }
}