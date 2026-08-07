using System.Diagnostics;
using Principle.Contracts;

namespace Principle.Engine;

public class TickScheduler : ITickScheduler
{
    public Dictionary<string, ITickSchedule> TickSchedules { get; } = new Dictionary<string, ITickSchedule>();
    public int TickCount { get; private set; } = 0;
    
    public const int MaxTickRate = 128;

    private double _targetElapsedTime = 0.0;
    private double _accumulator = 0.0;
    private long _lastTimestamp = Stopwatch.GetTimestamp();

    public TickScheduler()
    {
        _targetElapsedTime = 1.0 / MaxTickRate;
    }

    public void Tick()
    {
        long currentTimestamp = Stopwatch.GetTimestamp();
        
        double deltaTime = Stopwatch.GetElapsedTime(_lastTimestamp, currentTimestamp).TotalSeconds;
        _lastTimestamp = currentTimestamp;

        if (deltaTime > 0.25)
        {
            deltaTime = 0.25;
        }

        _accumulator += deltaTime;

        if (_accumulator >= _targetElapsedTime)
        {
            foreach (ITickSchedule tickSchedule in TickSchedules.Values)
            {
                tickSchedule.Tick();
            }

            TickCount++;
            _accumulator -= _targetElapsedTime;
        }

        Thread.Sleep(1);
    }

    public void AddTickSchedule(ITickSchedule tickSchedule)
    {
        AddTickSchedule($"Schedule_{TickSchedules.Count}", tickSchedule);
    }

    public void AddTickSchedule(string scheduleName, ITickSchedule tickSchedule)
    {
        TickSchedules[scheduleName] = tickSchedule;
    }
}