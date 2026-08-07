using Principle.Contracts;
using System.Diagnostics;

namespace TestGameProject;

public class MyTickSchedule : ITickSchedule
{
    public void Tick()
    {
        Debug.WriteLine($"Full second");
    }
}