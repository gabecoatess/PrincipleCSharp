using Principle.Engine.Logging;
using Principle.Engine.Simulation;

namespace TestGameProject;

public class MyTickSchedule : ITickSchedule
{
    public void Tick() => EngineLog.Information($"Full second");
}
