using Principle.Contracts;
using System.Diagnostics;

namespace TestGameProject;

public class MyOtherTickSchedule : ITickSchedule
{
    public void Tick()
    {
        Debug.WriteLine("Half second");
    }
}