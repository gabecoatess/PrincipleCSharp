using Principle.API.Renderer;
using Silk.NET.OpenGL;
using System.Drawing;

namespace Principle.Renderer;

public sealed class GLRenderer(GL glContext) : IRenderer
{
    private bool _initialized = false;

    private GL _openGl = glContext;
    private uint _vertexBufferArray;
    private uint _elementBufferObject;
    private uint _activeShaderProgram;

    private const uint positionLoc = 0;

    public bool Initialized => _initialized;

    public unsafe void Initialize()
    {
        _initialized = true;

        SetClearColor(Color.YellowGreen);

        _vertexBufferArray = _openGl.GenVertexArray();
        _openGl.BindVertexArray(_vertexBufferArray);

        _elementBufferObject = _openGl.GenBuffer();
        _openGl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _elementBufferObject);

        var _activeVertexShader = _openGl.CreateShader(ShaderType.VertexShader);
        var _activeFragmentShader = _openGl.CreateShader(ShaderType.FragmentShader);

        try
        {
            var assembly = typeof(GLRenderer).Assembly;
            using var stream = assembly.GetManifestResourceStream("Principle.Renderer.Shaders.base_vertex.glsl");
            using var reader = new StreamReader(stream!);
            string vertexShaderSource = reader.ReadToEnd();
            _openGl.ShaderSource(_activeVertexShader, vertexShaderSource);
            _openGl.CompileShader(_activeVertexShader);

            if (ShaderCompilationSuccessful(_activeVertexShader) == false)
            {
                throw new Exception(_openGl.GetShaderInfoLog(_activeVertexShader));
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Unable to load vertex shader: {ex}");
        }

        try
        {
            var assembly = typeof(GLRenderer).Assembly;
            using var stream = assembly.GetManifestResourceStream("Principle.Renderer.Shaders.base_fragment.glsl");
            using var reader = new StreamReader(stream!);
            string fragmentShaderSource = reader.ReadToEnd();
            _openGl.ShaderSource(_activeFragmentShader, fragmentShaderSource);
            _openGl.CompileShader(_activeFragmentShader);

            if (ShaderCompilationSuccessful(_activeFragmentShader) == false)
            {
                throw new Exception(_openGl.GetShaderInfoLog(_activeVertexShader));
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Unable to load fragment shader: {ex}");
        }

        _activeShaderProgram = _openGl.CreateProgram();
        _openGl.AttachShader(_activeShaderProgram, _activeVertexShader);
        _openGl.AttachShader(_activeShaderProgram, _activeFragmentShader);

        _openGl.LinkProgram(_activeShaderProgram);

        _openGl.GetProgram(_activeShaderProgram, ProgramPropertyARB.LinkStatus, out int lStatus);
        if (lStatus != (int)GLEnum.True)
        {
            throw new Exception(_openGl.GetProgramInfoLog(_activeShaderProgram));
        }

        _openGl.DetachShader(_activeShaderProgram, _activeVertexShader);
        _openGl.DetachShader(_activeShaderProgram, _activeFragmentShader);
        _openGl.DeleteShader(_activeVertexShader);
        _openGl.DeleteShader(_activeFragmentShader);

        _openGl.EnableVertexAttribArray(positionLoc);
        _openGl.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*)0);

        _openGl.BindVertexArray(0);
        _openGl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        _openGl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
    }

    public unsafe void Render()
    {
        if (_initialized == false)
        {
            throw new RendererNotInitializedException();
        }

        _openGl.BindVertexArray(_vertexBufferArray);
        _openGl.UseProgram(_activeShaderProgram);
        _openGl.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (void*)0);
    }

    public void SetClearColor(Color clearColor)
    {
        if (_initialized == false)
        {
            throw new RendererNotInitializedException();
        }

        _openGl.ClearColor(clearColor);
    }

    public void ClearColor ()
    {
        if (_initialized == false)
        {
            throw new RendererNotInitializedException();
        }

        _openGl.Clear(ClearBufferMask.ColorBufferBit);
    }

    public uint CreateVBO()
    {
        if (_initialized == false)
        {
            throw new RendererNotInitializedException();
        }

        return _openGl.GenBuffer();
    }

    public void BindVBO(uint vbo)
    {
        if (_initialized == false)
        {
            throw new RendererNotInitializedException();
        }

        _openGl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
    }

    public unsafe void AddBufferData(BufferTargetARB target, nuint size, void* data, GLEnum usage)
    {
        if (_initialized == false)
        {
            throw new RendererNotInitializedException();
        }

        _openGl.BufferData(target, size, data, usage);
    }

    public bool ShaderCompilationSuccessful(uint shaderObj)
    {
        if (_initialized == false)
        {
            throw new RendererNotInitializedException();
        }

        _openGl.GetShader(shaderObj, ShaderParameterName.CompileStatus, out int vStatus);
        return vStatus == (int)GLEnum.True;
    }
}