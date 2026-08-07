using Principle.Contracts;

namespace Principle.Engine;

internal sealed class ScheduledTick
{
    public required ITickSchedule Schedule { get; init; }
    public double Accumulator { get; set; }
}