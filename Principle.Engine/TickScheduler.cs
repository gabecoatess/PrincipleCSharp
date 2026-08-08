using System.Diagnostics;
using Principle.Contracts;

namespace Principle.Engine;

public class TickScheduler
{
    public int TickCount { get; private set; }

    public const double MaxTickRate = 128.0;

    private const double TargetElapsedTime = 1.0 / MaxTickRate;
    private double _accumulator;
    private long _lastTimestamp;

    private readonly Dictionary<string, ScheduledTick> _scheduledTicks = [];

    public void Start()
    {
        _lastTimestamp = Stopwatch.GetTimestamp();
        _accumulator = 0.0;

        foreach (var tick in _scheduledTicks.Values)
        {
            tick.Accumulator = 0.0;
        }
    }

    public void Tick()
    {
        var currentTimestamp = Stopwatch.GetTimestamp();

        var deltaTime = Stopwatch.GetElapsedTime(_lastTimestamp, currentTimestamp).TotalSeconds;
        _lastTimestamp = currentTimestamp;

        if (deltaTime > 0.25)
        {
            deltaTime = 0.25;
        }

        _accumulator += deltaTime;

        while (_accumulator >= TargetElapsedTime)
        {
            RunSchedulerTick(TargetElapsedTime);

            TickCount++;
            _accumulator -= TargetElapsedTime;
        }

        Thread.Sleep(1);
    }

    public void AddTickSchedule(string scheduleName, ITickSchedule tickSchedule, double tickRate = 20.0, bool overwrite = false)
    {
        if (tickRate is <= 0.0 or > MaxTickRate)
        {
            throw new ArgumentOutOfRangeException(nameof(tickRate), $"Tick rate must be greater than 0 and less than or equal to {MaxTickRate}.");
        }

        if (string.IsNullOrWhiteSpace(scheduleName))
        {
            throw new ArgumentException("Schedule name cannot be null or whitespace.", nameof(scheduleName));
        }

        if (!overwrite && _scheduledTicks.ContainsKey(scheduleName))
        {
            throw new InvalidOperationException("A schedule with the same name already exists.");
        }

        _scheduledTicks[scheduleName] = new ScheduledTick { Schedule = tickSchedule, TickRate = tickRate };
    }

    public bool TryGetTickSchedule(string scheduleName, out ITickSchedule? tickSchedule)
    {
        if (_scheduledTicks.TryGetValue(scheduleName, out var scheduledTick))
        {
            tickSchedule = scheduledTick.Schedule;
            return true;
        }

        tickSchedule = null;
        return false;
    }

    public bool RemoveTickSchedule(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Schedule name cannot be null or whitespace.", nameof(name));
        }

        return _scheduledTicks.Remove(name);
    }

    public double GetTickScheduleTickRate(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Schedule name cannot be null or whitespace.", nameof(name));
        }

        if (_scheduledTicks.TryGetValue(name, out var scheduledTick))
        {
            return scheduledTick.TickRate;
        }

        throw new InvalidOperationException("Schedule not found.");
    }

    private void RunSchedulerTick(double elapsedSeconds)
    {
        foreach (var scheduledTick in _scheduledTicks.Values)
        {
            var schedule = scheduledTick.Schedule;

            var scheduleInterval = 1.0 / scheduledTick.TickRate;
            scheduledTick.Accumulator += elapsedSeconds;

            while (scheduledTick.Accumulator >= scheduleInterval)
            {
                schedule.Tick();
                scheduledTick.Accumulator -= scheduleInterval;
            }
        }
    }
}
