using Principle.Engine;

namespace TestGameProject;

public class MyGame : PrincipleGame
{
    protected override void OnInitialize()
    {
        TickScheduler.AddTickSchedule("MyTickSchedule", new MyTickSchedule(), 1);
        TickScheduler.AddTickSchedule("MyTickSchedule2", new MyOtherTickSchedule(), 2);
    }
}
