using System.Diagnostics;
using Principle.Contracts;

namespace Principle.Engine;

public class TickScheduler
{
    public int TickCount { get; private set; }

    public const int MaxTickRate = 128;

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

    public void AddTickSchedule(string scheduleName, ITickSchedule tickSchedule, int tickRate = 20)
    {
        if (tickRate is < 1 or > MaxTickRate)
        {
            throw new ArgumentOutOfRangeException(nameof(tickRate), $"Tick rate must be between 1 and {MaxTickRate}");
        }

        _scheduledTicks[scheduleName] = new ScheduledTick { Schedule = tickSchedule, TickRate = tickRate };
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
