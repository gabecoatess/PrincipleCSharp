using System.Diagnostics;
using Principle.Contracts;

namespace Principle.Engine;

public class TickScheduler
{
    public int TickCount { get; private set; } = 0;
    
    public const int MaxTickRate = 128;

    private double _targetElapsedTime = 0.0;
    private double _accumulator = 0.0;
    private long _lastTimestamp = 0L;

    private Dictionary<string, ScheduledTick> _scheduledTicks = new Dictionary<string, ScheduledTick>();

    public TickScheduler()
    {
        _targetElapsedTime = 1.0 / MaxTickRate;
    }

    public void Start()
    {
        _lastTimestamp = Stopwatch.GetTimestamp();
        _accumulator = 0.0;

        foreach (ScheduledTick tick in _scheduledTicks.Values)
        {
            tick.Accumulator = 0.0;
        }
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

        while (_accumulator >= _targetElapsedTime)
        {
            RunSchedulerTick(_targetElapsedTime);

            TickCount++;
            _accumulator -= _targetElapsedTime;
        }

        Thread.Sleep(1);
    }

    public void AddTickSchedule(ITickSchedule tickSchedule, int tickRate = 20)
    {
        AddTickSchedule($"Schedule_{_scheduledTicks.Count}", tickSchedule, tickRate);
    }

    public void AddTickSchedule(string scheduleName, ITickSchedule tickSchedule, int tickRate = 20)
    {
        if (tickRate < 1 || tickRate > MaxTickRate)
        {
            throw new ArgumentOutOfRangeException(nameof(tickRate), $"Tick rate must be between 1 and {MaxTickRate}");
        }

        _scheduledTicks[scheduleName] = new ScheduledTick { Schedule = tickSchedule, tickRate = tickRate };
    }

    private void RunSchedulerTick(double elapsedSeconds)
    {
        foreach (ScheduledTick scheduledTick in _scheduledTicks.Values)
        {
            ITickSchedule schedule = scheduledTick.Schedule;

            double scheduleInterval = 1.0 / scheduledTick.tickRate;
            scheduledTick.Accumulator += elapsedSeconds;

            while (scheduledTick.Accumulator >= scheduleInterval)
            {
                schedule.Tick();
                scheduledTick.Accumulator -= scheduleInterval;
            }
        }
    }
}