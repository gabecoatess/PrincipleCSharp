namespace Principle.Engine;

public interface ITickSchedule
{
    int TickRate { get; set; }
    void Tick();
}