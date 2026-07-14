using System.Numerics;
using Veldrid;

namespace Principle.Renderer;

public struct VertexPositionColor
{
    public Vector2 Position;
    public RgbaFloat Color;

    public const uint SIZE_IN_BYTES = 24;
    
    public VertexPositionColor(Vector2 position, RgbaFloat color)
    {
        Position = position;
        Color = color;
    }
}