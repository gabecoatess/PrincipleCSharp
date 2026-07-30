using System.Collections.Immutable;

namespace Principle.Rendering.Abstractions;

public readonly record struct RenderTargetHandle(ulong Value)
{
    public bool IsValid => Value != 0;
}

public readonly record struct RenderTargetDescription(int Width, int Height);

public readonly record struct RenderSurfaceSize(
    int LogicalWidth,
    int LogicalHeight,
    int DrawableWidth,
    int DrawableHeight);

public readonly record struct RenderColor(byte R, byte G, byte B, byte A = byte.MaxValue);

public readonly record struct RenderRectangle(float X, float Y, float Width, float Height);

public abstract record RenderCommand
{
    private protected RenderCommand()
    {
    }
}

public sealed record ClearTargetCommand(RenderColor Color) : RenderCommand;

public sealed record DrawRectangleCommand(RenderRectangle Bounds, RenderColor Color) : RenderCommand;

public sealed record RenderFrame(ImmutableArray<RenderCommand> Commands)
{
    public static RenderFrame Create(params RenderCommand[] commands)
    {
        ArgumentNullException.ThrowIfNull(commands);
        return new RenderFrame(ImmutableArray.CreateRange(commands));
    }
}

public sealed class RenderImage
{
    public RenderImage(int width, int height, ImmutableArray<RenderColor> pixels)
    {
        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width));
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height));
        }

        if (pixels.IsDefault || pixels.Length != checked(width * height))
        {
            throw new ArgumentException("The pixel count must match the image dimensions.", nameof(pixels));
        }

        Width = width;
        Height = height;
        Pixels = pixels;
    }

    public int Width { get; }

    public int Height { get; }

    public ImmutableArray<RenderColor> Pixels { get; }

    public RenderColor GetPixel(int x, int y)
    {
        if ((uint)x >= (uint)Width)
        {
            throw new ArgumentOutOfRangeException(nameof(x));
        }

        if ((uint)y >= (uint)Height)
        {
            throw new ArgumentOutOfRangeException(nameof(y));
        }

        return Pixels[checked(y * Width + x)];
    }
}
