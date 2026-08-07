using Principle.Engine;

namespace Principle.Contracts;

public interface ITickScheduler
{
    Dictionary<string, ITickSchedule>  TickSchedules { get; }
    int TickCount { get; }
    
    void Tick();
    void AddTickSchedule(string scheduleName, ITickSchedule tickSchedule);
}