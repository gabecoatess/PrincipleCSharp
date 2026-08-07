namespace Principle.Contracts;

public interface ITickSchedule
{
    int TickRate { get; set; }
    void Tick();
}