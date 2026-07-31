namespace Principle.Editor.Models;

public sealed class ProjectCacheModel
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Path { get; set; }
    public required int Major { get; set; }
    public required int Minor { get; set; }
    public required int Patch { get; set; }
}