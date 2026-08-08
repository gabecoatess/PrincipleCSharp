using System.Diagnostics;
using Principle.Contracts;

namespace TestGameProject;

public class MyTickSchedule : ITickSchedule
{
    public void Tick() => Debug.WriteLine($"Full second");
}
