using System.Diagnostics;
using Principle.Engine;

namespace TestGameProject;

public class MyOtherTickSchedule : ITickSchedule
{
    private int _counter;

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
