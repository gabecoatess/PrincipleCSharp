namespace Principle.Renderer;

public class RendererNotInitializedException : Exception
{
    public RendererNotInitializedException() : base() { }

    public RendererNotInitializedException(string message) : base(message) { }

    public RendererNotInitializedException(string message, Exception innerException)
        : base(message, innerException) { }
}