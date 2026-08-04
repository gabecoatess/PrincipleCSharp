namespace Principle.Engine;

public class TickScheduler
{
    private Dictionary<string, TickSchedule> _tickSchedules = new Dictionary<string, TickSchedule>();

    public bool IsTicking { get; private set; } = false;
    
    public void RunTickSchedules()
    {
        foreach (TickSchedule tickSchedule in _tickSchedules.Values)
        {
            tickSchedule.Tick();
        }
    }

    public void AddTickSchedule()
    {
        AddTickSchedule($"Schedule_{_tickSchedules.Count}", new TickSchedule());
    }
    
    public void AddTickSchedule(string scheduleName)
    {
        AddTickSchedule(scheduleName, new TickSchedule());
    }

    public void AddTickSchedule(int tickRate)
    {
        AddTickSchedule($"Schedule_{_tickSchedules.Count}", new TickSchedule(tickRate));
    }

    public void AddTickSchedule(string scheduleName, int tickRate)
    {
        AddTickSchedule(scheduleName, new TickSchedule(tickRate));
    }

    public void AddTickSchedule(string scheduleName, TickSchedule tickSchedule)
    {
        _tickSchedules[scheduleName] = tickSchedule;
    }
}