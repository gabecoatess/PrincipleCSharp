using System.Diagnostics;
using Principle.ECS;

namespace TestGameProject;

public class MyExampleSystem : IPrincipleSystem
{
    public void Tick() => Debug.WriteLine($"Full second");
}
