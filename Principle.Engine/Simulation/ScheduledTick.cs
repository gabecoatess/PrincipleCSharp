namespace Principle.Engine.Simulation;

internal sealed class ScheduledTick
{
    public required ITickSchedule Schedule { get; init; }
    public double TickRate { get; init; }
    public double Accumulator { get; set; }
}
