using Principle.Platform;
using Principle.Rendering;
using Principle.Rendering.Abstractions;
using Principle.Rendering.Raylib;

namespace Principle.Sandbox;

internal static class Program
{
    private const int Success = 0;
    private const int Failure = 1;

    public static int Main(string[] args)
    {
        var arguments = SandboxArguments.Parse(args);
        if (!arguments.IsSuccess)
        {
            Console.Error.WriteLine(arguments.Error);
            return Failure;
        }

        var options = arguments.Value!;
        return options.Mode switch
        {
            SandboxMode.Window => RunWindow(visible: true, frameLimit: null),
            SandboxMode.VerifyWindow => RunWindow(visible: false, frameLimit: 3),
            SandboxMode.Offscreen => RunOffscreen(options.OutputPath),
            _ => Failure
        };
    }

    private static int RunWindow(bool visible, int? frameLimit)
    {
        var created = RaylibBackend.CreateSession(new WindowDescription(
            "Principle Rendering Sandbox",
            800,
            600,
            IsResizable: true,
            IsVisible: visible));

        if (!created.IsSuccess)
        {
            Write(created.Error!);
            return Failure;
        }

        using var session = created.Value;
        var exitCode = Success;
        var renderedFrames = 0;

        try
        {
            while (frameLimit is null || renderedFrames < frameLimit.Value)
            {
                var polled = session.Window.PollEvents();
                if (!polled.IsSuccess)
                {
                    Write(polled.Error!);
                    exitCode = Failure;
                    break;
                }

                if (polled.Value.CloseRequested)
                {
                    break;
                }

                var resize = session.ApplyWindowResize(polled.Value);
                if (!resize.IsSuccess)
                {
                    Write(resize.Error!);
                    exitCode = Failure;
                    break;
                }

                var rendered = session.RenderToWindow(SandboxFrame.Create());
                if (!rendered.IsSuccess)
                {
                    Write(rendered.Error!);
                    exitCode = Failure;
                    break;
                }

                renderedFrames++;
            }
        }
        finally
        {
            if (!WriteShutdownFailures(session.Shutdown()))
            {
                exitCode = Failure;
            }
        }

        return exitCode;
    }

    private static int RunOffscreen(string outputPath)
    {
        var created = RaylibBackend.CreateSession(new WindowDescription(
            "Principle Offscreen Context",
            256,
            256,
            IsResizable: false,
            IsVisible: false));

        if (!created.IsSuccess)
        {
            Write(created.Error!);
            return Failure;
        }

        using var session = created.Value;
        var exitCode = Success;
        RenderTargetHandle offscreenTarget = default;

        try
        {
            var target = session.Renderer.CreateOffscreenTarget(new RenderTargetDescription(256, 256));
            if (!target.IsSuccess)
            {
                Write(target.Error!);
                return Failure;
            }

            offscreenTarget = target.Value;

            var submitted = session.Renderer.Submit(offscreenTarget, SandboxFrame.Create());
            if (!submitted.IsSuccess)
            {
                Write(submitted.Error!);
                return Failure;
            }

            var readback = session.Renderer.ReadRenderTarget(offscreenTarget);
            if (!readback.IsSuccess)
            {
                Write(readback.Error!);
                return Failure;
            }

            var background = readback.Value.GetPixel(8, 8);
            var rectangle = readback.Value.GetPixel(80, 80);
            if (background != SandboxFrame.BackgroundColor ||
                rectangle != SandboxFrame.RectangleColor)
            {
                Console.Error.WriteLine(
                    $"Offscreen verification failed. Background={background}; Rectangle={rectangle}.");
                return Failure;
            }

            var fullOutputPath = Path.GetFullPath(outputPath);
            var directory = Path.GetDirectoryName(fullOutputPath);
            if (string.IsNullOrEmpty(directory))
            {
                Console.Error.WriteLine("Could not determine the PNG output directory.");
                return Failure;
            }

            Directory.CreateDirectory(directory);
            var saved = session.Renderer.SaveRenderTargetPng(offscreenTarget, fullOutputPath);
            if (!saved.IsSuccess)
            {
                Write(saved.Error!);
                return Failure;
            }

            var file = new FileInfo(fullOutputPath);
            if (!file.Exists || file.Length == 0)
            {
                Console.Error.WriteLine("The offscreen PNG was not created or is empty.");
                return Failure;
            }

            Console.WriteLine($"Verified offscreen render and saved {fullOutputPath}");
        }
        finally
        {
            if (offscreenTarget.IsValid)
            {
                var destroyed = session.Renderer.DestroyRenderTarget(offscreenTarget);
                if (!destroyed.IsSuccess)
                {
                    Write(destroyed.Error!);
                    exitCode = Failure;
                }
            }

            if (!WriteShutdownFailures(session.Shutdown()))
            {
                exitCode = Failure;
            }
        }

        return exitCode;
    }

    private static bool WriteShutdownFailures(ShutdownReport report)
    {
        if (report.RendererError is not null)
        {
            Write(report.RendererError);
        }

        if (report.WindowError is not null)
        {
            Write(report.WindowError);
        }

        return report.IsSuccess;
    }

    private static void Write(RenderError error)
    {
        Console.Error.WriteLine(
            $"{error.Code}: {error.Message}{FormatDetail(error.Detail)}");
    }

    private static void Write(PlatformError error)
    {
        Console.Error.WriteLine(
            $"{error.Code}: {error.Message}{FormatDetail(error.Detail)}");
    }

    private static string FormatDetail(string? detail) =>
        string.IsNullOrWhiteSpace(detail) ? string.Empty : $" ({detail})";
}

internal enum SandboxMode
{
    Window,
    VerifyWindow,
    Offscreen
}

internal sealed record SandboxOptions(SandboxMode Mode, string OutputPath);

internal readonly record struct ArgumentResult(SandboxOptions? Value, string? Error)
{
    public bool IsSuccess => Error is null;

    public static ArgumentResult Success(SandboxOptions value) => new(value, null);

    public static ArgumentResult Failure(string error) => new(null, error);
}

internal static class SandboxArguments
{
    private static readonly string DefaultOutputPath =
        Path.Combine("artifacts", "Principle.Sandbox", "offscreen.png");

    public static ArgumentResult Parse(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        var mode = SandboxMode.Window;
        var outputPath = DefaultOutputPath;

        for (var index = 0; index < args.Length; index++)
        {
            switch (args[index])
            {
                case "--window":
                    mode = SandboxMode.Window;
                    break;
                case "--verify-window":
                    mode = SandboxMode.VerifyWindow;
                    break;
                case "--offscreen":
                    mode = SandboxMode.Offscreen;
                    break;
                case "--output" when index + 1 < args.Length:
                    outputPath = args[++index];
                    break;
                case "--output":
                    return ArgumentResult.Failure("--output requires a path.");
                default:
                    return ArgumentResult.Failure($"Unknown argument: {args[index]}");
            }
        }

        return ArgumentResult.Success(new SandboxOptions(mode, outputPath));
    }
}
