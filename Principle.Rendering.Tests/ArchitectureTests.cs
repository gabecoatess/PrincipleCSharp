using System.Reflection;
using Principle.Platform;
using Principle.Rendering.Abstractions;

namespace Principle.Rendering.Tests;

public sealed class ArchitectureTests
{
    [Fact]
    public void NeutralPublicApisDoNotExposeRaylibTypes()
    {
        Assembly[] neutralAssemblies =
        [
            typeof(IPlatformWindow).Assembly,
            typeof(IRenderer).Assembly,
            typeof(RenderSession).Assembly
        ];

        foreach (var assembly in neutralAssemblies)
        {
            foreach (var type in assembly.GetExportedTypes())
            {
                AssertNotRaylib(type);

                foreach (var constructor in type.GetConstructors())
                {
                    foreach (var parameter in constructor.GetParameters())
                    {
                        AssertNotRaylib(parameter.ParameterType);
                    }
                }

                foreach (var property in type.GetProperties())
                {
                    AssertNotRaylib(property.PropertyType);
                }

                foreach (var method in type.GetMethods())
                {
                    AssertNotRaylib(method.ReturnType);
                    foreach (var parameter in method.GetParameters())
                    {
                        AssertNotRaylib(parameter.ParameterType);
                    }
                }
            }
        }
    }

    [Fact]
    public void OnlyBackendProjectReferencesRaylibPackage()
    {
        var repository = FindRepositoryRoot();
        var projectFiles = Directory.GetFiles(repository, "*.csproj", SearchOption.AllDirectories)
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}"))
            .ToArray();

        var raylibReferences = projectFiles
            .Where(path => File.ReadAllText(path).Contains("PackageReference Include=\"Raylib-cs\""))
            .Select(path => Path.GetFileNameWithoutExtension(path)!)
            .ToArray();

        Assert.Equal("Principle.Rendering.Raylib", Assert.Single(raylibReferences));
    }

    [Fact]
    public void ProductionProjectsDoNotReferenceSandbox()
    {
        var repository = FindRepositoryRoot();
        var projectFiles = Directory.GetFiles(repository, "*.csproj", SearchOption.AllDirectories)
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}"))
            .Where(path => Path.GetFileNameWithoutExtension(path) != "Principle.Sandbox");

        foreach (var project in projectFiles)
        {
            Assert.DoesNotContain(
                "Principle.Sandbox.csproj",
                File.ReadAllText(project),
                StringComparison.Ordinal);
        }
    }

    private static void AssertNotRaylib(Type type)
    {
        if (type.IsArray || type.IsByRef || type.IsPointer)
        {
            AssertNotRaylib(type.GetElementType()!);
            return;
        }

        Assert.False(
            string.Equals(type.Namespace, "Raylib_cs", StringComparison.Ordinal),
            $"Public API exposed Raylib type {type.FullName}.");

        if (!type.IsGenericType)
        {
            return;
        }

        foreach (var argument in type.GetGenericArguments())
        {
            AssertNotRaylib(argument);
        }
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "PrincipleCSharp.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the repository root.");
    }
}
