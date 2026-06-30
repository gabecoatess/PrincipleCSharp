using Silk.NET.OpenGL;

namespace EngineAPI;

public class Triangle(uint vbo)
{
    public static float[] TriangleVertices =
    {
         0.5f,  0.5f, 0.0f,
         0.5f, -0.5f, 0.0f,
        -0.5f, -0.5f, 0.0f,
        -0.5f,  0.5f, 0.0f
    };

    public static uint[] TriangleIndices =
    {
        0u, 1u, 3u,
        1u, 2u, 3u
    };

    private uint _vertexBufferObject = vbo;

    public static unsafe Triangle New(Renderer renderer)
    {
        var vbo = renderer.CreateVBO();
        renderer.BindVBO(vbo);

        fixed (float* buffer = TriangleVertices)
        {
            renderer.AddBufferData(BufferTargetARB.ArrayBuffer, (nuint)(TriangleVertices.Length * sizeof(float)), buffer, (GLEnum)BufferUsageARB.StaticDraw);
        }

        fixed (uint* buffer = TriangleIndices)
        {
            renderer.AddBufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(TriangleIndices.Length * sizeof(uint)), buffer, (GLEnum)BufferUsageARB.StaticDraw);
        }

        return new Triangle(vbo);
    }
}