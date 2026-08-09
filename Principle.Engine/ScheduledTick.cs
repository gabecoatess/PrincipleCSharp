namespace Principle.Engine;

internal sealed class ScheduledTick
{
    public required TickSchedule Schedule { get; init; }
    public double TickRate { get; init; }
    public double Accumulator { get; set; }
}
