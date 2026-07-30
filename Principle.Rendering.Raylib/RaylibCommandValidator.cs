using Principle.Rendering.Abstractions;

namespace Principle.Rendering.Raylib;

internal static class RaylibCommandValidator
{
    public static RenderResult Validate(RenderFrame? frame)
    {
        if (frame is null || frame.Commands.IsDefault)
        {
            return RenderResult.Failure(
                RenderErrorCode.InvalidFrame,
                "The render frame or its command collection is invalid.");
        }

        foreach (var command in frame.Commands)
        {
            if (command is null)
            {
                return RenderResult.Failure(
                    RenderErrorCode.InvalidFrame,
                    "A render frame cannot contain a null command.");
            }

            if (command is not DrawRectangleCommand rectangle)
            {
                continue;
            }

            var bounds = rectangle.Bounds;
            if (!float.IsFinite(bounds.X) ||
                !float.IsFinite(bounds.Y) ||
                !float.IsFinite(bounds.Width) ||
                !float.IsFinite(bounds.Height) ||
                bounds.Width <= 0 ||
                bounds.Height <= 0)
            {
                return RenderResult.Failure(
                    RenderErrorCode.InvalidFrame,
                    "Rectangle coordinates must be finite and dimensions must be positive.");
            }
        }

        return RenderResult.Success();
    }
}
