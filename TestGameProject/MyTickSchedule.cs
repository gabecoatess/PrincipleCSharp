using System.Diagnostics;
using Principle.Engine.Simulation;

namespace TestGameProject;

public class MyTickSchedule : ITickSchedule
{
    public void Tick() => Debug.WriteLine($"Full second");
}
