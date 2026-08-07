using Principle.Engine;

namespace TestGameProject;

public class MyTickSchedule : ITickSchedule
{
    public int TickRate { get; set; } = 20;

    private int _tickCount = 0;

    public void Tick()
    {
        Console.WriteLine($"[{_tickCount}] TickRate: {TickRate}");
        _tickCount++;

        if (_tickCount >= 5)
        {
            Application.Quit();
        }
    }
}