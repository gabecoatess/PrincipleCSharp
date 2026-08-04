namespace Principle.Engine;

public class TickSchedule(int tickRate = 20)
{
    public int TickRate { get; set; } = tickRate;

    public void Tick()
    {
        Console.WriteLine("Tick");
    }
}