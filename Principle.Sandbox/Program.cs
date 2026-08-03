using Principle.Platform;
using Principle.Rendering;
using Principle.Rendering.Abstractions;
using Principle.Rendering.Raylib;

namespace Principle.Sandbox;

internal static class Program
{
    public static int Main()
    {
        var created = RaylibBackend.CreateSession(new WindowDescription(
            "Principle Rendering Sandbox",
            800,
            600,
            IsResizable: true,
            IsVisible: true));

        if (!created.IsSuccess)
        {
            LogError(created.Error!);
            return 1;
        }

        using var session = created.Value;
        var exitCode = 0;

        try
        {
            while (true)
            {
                var polled = session.Window.PollEvents();
                if (!polled.IsSuccess)
                {
                    LogError(polled.Error!);
                    exitCode = 1;
                    break;
                }

                if (polled.Value.CloseRequested)
                {
                    break;
                }

                var resize = session.ApplyWindowResize(polled.Value);
                if (!resize.IsSuccess)
                {
                    LogError(resize.Error!);
                    exitCode = 1;
                    break;
                }

                var rendered = session.RenderToWindow(SandboxFrame.Create());
                if (!rendered.IsSuccess)
                {
                    LogError(rendered.Error!);
                    exitCode = 1;
                    break;
                }
            }
        }
        finally
        {
            var report = session.Shutdown();
            if (report.RendererError is not null)
            {
                LogError(report.RendererError);
            }

            if (report.WindowError is not null)
            {
                LogError(report.WindowError);
            }

            if (!report.IsSuccess)
            {
                exitCode = 1;
            }
        }

        return exitCode;
    }

    private static void LogError(RenderError error)
    {
        Console.Error.WriteLine($"{error.Code}: {error.Message}{FormatDetail(error.Detail)}");
    }
    
    private static void LogError(PlatformError error)
    {
        Console.Error.WriteLine($"{error.Code}: {error.Message}{FormatDetail(error.Detail)}");
    }
    
    private static string FormatDetail(string? detail)
    {
        return string.IsNullOrWhiteSpace(detail) ? string.Empty : $" ({detail})";
    }
}
