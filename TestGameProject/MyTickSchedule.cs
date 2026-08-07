using Principle.Contracts;
using System.Diagnostics;

namespace TestGameProject;

public class MyTickSchedule : ITickSchedule
{
    public int TickRate { get; set; } = 1;

    public void Tick()
    {
        Debug.WriteLine($"Full second");
    }
}