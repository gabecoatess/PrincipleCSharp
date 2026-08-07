using Principle.Contracts;
using Principle.Engine;
using System.Diagnostics;

namespace TestGameProject;

public class MyOtherTickSchedule : ITickSchedule
{
    private int _counter = 0;

    public void Tick()
    {
        _counter++;

        if (_counter % 5 == 0)
        {
            Application.Quit();
        }
        Debug.WriteLine("Half second");
    }
}