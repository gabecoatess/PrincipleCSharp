using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Drawing;

namespace EngineAPI;

public class Renderer
{
    private GL? _openGl;
    private uint _vertexBufferArray;
    private uint _elementBufferObject;
    private uint _activeShaderProgram;

    private const uint positionLoc = 0;

    public unsafe Renderer(IWindow window)
    {
        _openGl = window.CreateOpenGL();
        _vertexBufferArray = _openGl.GenVertexArray();
        _openGl.BindVertexArray(_vertexBufferArray);

        _elementBufferObject = _openGl.GenBuffer();
        _openGl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _elementBufferObject);

        Triangle.New(this);

        var _activeVertexShader = _openGl.CreateShader(ShaderType.VertexShader);
        var _activeFragmentShader = _openGl.CreateShader(ShaderType.FragmentShader);

        try
        {
            string vertexShaderSource = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "assets/shaders/base_vertex.glsl"));
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
            string fragmentShaderSource = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "assets/shaders/base_fragment.glsl"));
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
        _openGl!.BindVertexArray(_vertexBufferArray);
        _openGl!.UseProgram(_activeShaderProgram);
        _openGl!.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (void*)0);
    }

    public void SetClearColor(Color clearColor)
    {
        if (_openGl == null)
        {
            throw new Exception("OpenGL has not been initialized!");
        }

        _openGl?.ClearColor(clearColor);
    }

    public void ClearColorBufferBit()
    {
        if (_openGl == null)
        {
            throw new Exception("OpenGL has not been initialized!");
        }

        _openGl?.Clear(ClearBufferMask.ColorBufferBit);
    }

    public uint CreateVBO()
    {
        if (_openGl == null)
        {
            throw new Exception("OpenGL has not been initialized!");
        }

        return _openGl.GenBuffer();
    }

    public void BindVBO(uint vbo)
    {
        if (_openGl == null)
        {
            throw new Exception("OpenGL has not been initialized!");
        }

        _openGl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
    }

    public unsafe void AddBufferData(BufferTargetARB target, nuint size, void* data, GLEnum usage)
    {
        if (_openGl == null)
        {
            throw new Exception("OpenGL has not been initialized!");
        }

        _openGl.BufferData(target, size, data, usage);
    }

    public bool ShaderCompilationSuccessful(uint shaderObj)
    {
        if (_openGl == null)
        {
            throw new Exception("OpenGL has not been initialized!");
        }

        _openGl.GetShader(shaderObj, ShaderParameterName.CompileStatus, out int vStatus);
        return vStatus == (int)GLEnum.True;
    }
}