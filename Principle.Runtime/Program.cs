using System.Diagnostics;
using Principle.Engine;
using Principle.Rendering.Abstractions;
using Principle.Rendering.Raylib;
using TestGameProject;

namespace Principle.Runtime;

public static class Program
{
    private static bool _openWindow = true;

    public static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            ParseArgs(args);
        }

        var host = new EngineHost();

        var engineHostThread = new Thread(() => host.Run(new MyGame()));
        engineHostThread.Start();

        if (_openWindow)
        {
            var windowCreated = false;
            var window = RaylibBackend.CreateSession(new Platform.WindowDescription(
                "Principle Engine", 800, 600, IsResizable: true, IsVisible: true));

            if (window.IsSuccess)
            {
                throw new Exception("Failed to create window: " + window.Error?.Message);
            }

            windowCreated = true;

            using var session = window.Value;
            var shouldClose = false;

            while (!shouldClose)
            {
                var polled = session.Window.PollEvents();
                if (!polled.IsSuccess)
                {
                    throw new Exception("Failed to poll events: " + polled.Error?.Message);
                }

                var resize = session.ApplyWindowResize(polled.Value);
                if (!resize.IsSuccess)
                {
                    throw new Exception("Failed to apply window resize: " + resize.Error?.Message);
                }

                var rendered = session.RenderToWindow(RenderFrame.Create(new ClearTargetCommand(new RenderColor(24, 32, 48)), new DrawRectangleCommand(new RenderRectangle(64, 64, 96, 80), new RenderColor(224, 72, 88))));
                if (!rendered.IsSuccess)
                {
                    throw new Exception("Failed to render to window: " + rendered.Error?.Message);
                }

                shouldClose = polled.Value.CloseRequested;
            }

            if (windowCreated)
            {
                host.RequestShutdown();
            }

            engineHostThread.Join();
        }
        else
        {
            host.RequestShutdown();
            engineHostThread.Join();
        }

    }

    private static void ParseArgs(string[] args)
    {
        foreach (var arg in args)
        {
            Debug.WriteLine($"[Runtime] Used arg: '{arg}'");

            _openWindow = arg switch
            {
                "-nw" or "--no-window" => false,
                _ => _openWindow
            };
        }
    }
}
