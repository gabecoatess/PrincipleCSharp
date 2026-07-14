using Principle.API.Renderer;
using System.Drawing;
using Veldrid;

namespace Principle.Renderer;

public class VeldridRenderer : IRenderer
{
    private GraphicsDevice? _graphicsDevice = null;

    public void Initialize(IRenderSurface renderSurface)
    {
        GraphicsDeviceOptions gpuOptions = new GraphicsDeviceOptions
        {
            PreferStandardClipSpaceYDirection = true,
            PreferDepthRangeZeroToOne = true,
        };

        _graphicsDevice = GraphicsDevice.CreateVulkan(gpuOptions);
    }

    public void Render()
    {
        throw new NotImplementedException();
    }

    public void ClearColor()
    {
        throw new NotImplementedException();
    }

    public void SetClearColor(Color clearColor)
    {
        throw new NotImplementedException();
    }
}