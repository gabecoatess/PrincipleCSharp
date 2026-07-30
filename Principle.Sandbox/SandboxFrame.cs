using Principle.Rendering.Abstractions;

namespace Principle.Sandbox;

internal static class SandboxFrame
{
    public static readonly RenderColor BackgroundColor = new(24, 32, 48);
    public static readonly RenderColor RectangleColor = new(224, 72, 88);
    public static readonly RenderRectangle Rectangle = new(64, 64, 96, 80);

    public static RenderFrame Create() =>
        RenderFrame.Create(
            new ClearTargetCommand(BackgroundColor),
            new DrawRectangleCommand(Rectangle, RectangleColor));
}
