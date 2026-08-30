using Principle.Engine.Logging;
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

        EngineLog.Debug("Half second");
    }
}
