using System.Diagnostics;
using Principle.Engine;

namespace TestGameProject;

public class MyTickSchedule : ITickSchedule
{
    public void Tick() => Debug.WriteLine($"Full second");
}
