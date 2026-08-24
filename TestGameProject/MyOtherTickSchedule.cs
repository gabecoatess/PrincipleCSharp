using System.Diagnostics;
using Principle.Engine.Simulation;

namespace TestGameProject;

public class MyOtherTickSchedule : ITickSchedule
{
    private int _counter;

    public void Tick()
    {
        _counter++;

        if (_counter % 5 == 0)
        {
        }

        Debug.WriteLine("Half second");
    }
}
