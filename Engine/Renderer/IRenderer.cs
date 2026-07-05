using System.Drawing;

namespace Principle.API.Renderer;

public interface IRenderer
{
    void Initialize();
    void Render();
    void SetClearColor(Color clearColor);
    void ClearColor();
}