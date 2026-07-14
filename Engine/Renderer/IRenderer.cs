using System.Drawing;

namespace Principle.API.Renderer;

public interface IRenderer
{
    Color FrameClearColor { get; set; }

    void Initialize(IRenderSurface renderSurface);
    void PrepareFrame();
    void Render();
    void FinalizeFrame();
    void SetClearColor(Color clearColor);
    void ClearColor();
    void Dispose();
}