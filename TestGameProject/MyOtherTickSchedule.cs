using Principle.Contracts;
using System.Diagnostics;

namespace TestGameProject;

public class MyOtherTickSchedule : ITickSchedule
{
    public int TickRate { get; set; } = 2;

    public void Tick()
    {
        Debug.WriteLine("Half second");
    }
}