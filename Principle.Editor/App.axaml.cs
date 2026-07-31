using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Principle.Editor.ViewModels;
using Principle.Editor.Views;
using System.Reflection;

namespace Principle.Editor;

public partial class App : Application
{
    public static string EngineName => Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "Principle Engine";
    public static string EditorName => Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? "Principle Editor";
    public static string EditorVersion => Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "(unknown)";

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new ProjectHub
            {
                DataContext = new ProjectHubViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}