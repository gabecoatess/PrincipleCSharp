using Principle.API.Renderer;
using System.Drawing;
using System.Numerics;
using System.Text;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;

namespace Principle.Renderer;

public class VeldridRenderer : IRenderer, IDisposable
{
    private GraphicsDevice? _graphicsDevice = null;
    private CommandList? _commandList = null;
    private Pipeline? _graphicsPipeline = null;
    private ResourceFactory? _resourceFactory = null;

    private DeviceBuffer? _globalVertexBuffer = null;
    private DeviceBuffer? _globalIndexBuffer = null;

    private Shader[]? _shaderResources = null;

    private RgbaFloat _clearColor = new RgbaFloat(0, 0, 0, 1);

    public Color FrameClearColor 
    { 
        get => Color.FromArgb(
            (int)Math.Round(_clearColor.A * 255), 
            (int)Math.Round(_clearColor.R * 255), 
            (int)Math.Round(_clearColor.G * 255), 
            (int)Math.Round(_clearColor.B * 255)); 

        set => _clearColor = new RgbaFloat(
            value.R / 255.0f, 
            value.G / 255.0f, 
            value.B / 255.0f, 
            value.A / 255.0f); 
    }

    public bool Initialized => _graphicsDevice != null && 
        _commandList != null && _graphicsPipeline != null && _resourceFactory != null &&
        _globalVertexBuffer != null && _globalIndexBuffer != null;

    public void Initialize(IRenderSurface renderSurface)
    {
        GraphicsDeviceOptions gpuOptions = new GraphicsDeviceOptions
        {
            PreferStandardClipSpaceYDirection = true,
            PreferDepthRangeZeroToOne = true,
        };

        // Veldrid source
        // return GraphicsDevice.CreateVulkan(swapchainDescription: new SwapchainDescription(GetSwapchainSource(window), (uint)window.Width, (uint)window.Height, options.SwapchainDepthFormat, options.SyncToVerticalBlank, colorSrgb), options: options);
        // need to figure out how to not use SDL here, it assumes it anyways
        if (!(renderSurface.GetInternalSurface() is Sdl2Window window))
        {
            throw new Exception("Unable to create a renderer with anything other than an Sdl2Window at the moment!");
        }

        _graphicsDevice = GraphicsDevice.CreateVulkan(gpuOptions, new SwapchainDescription(VeldridStartup.GetSwapchainSource(window), (uint)window.Width, (uint)window.Height, gpuOptions.SwapchainDepthFormat, gpuOptions.SyncToVerticalBlank, true));
        _resourceFactory = _graphicsDevice.ResourceFactory;


        // ABSTRACT AWAY
        VertexPositionColor[] quadVertices =
        {
            new VertexPositionColor(new Vector2(-0.75f, 0.75f), RgbaFloat.Red),
            new VertexPositionColor(new Vector2(0.75f, 0.75f), RgbaFloat.Green),
            new VertexPositionColor(new Vector2(-0.75f, -0.75f), RgbaFloat.Blue),
            new VertexPositionColor(new Vector2(0.75f, -0.75f), RgbaFloat.Yellow)
        };

        ushort[] quadIndices = { 0, 1, 2, 3 };

        _globalVertexBuffer = _resourceFactory.CreateBuffer(new BufferDescription(VertexPositionColor.SIZE_IN_BYTES * (uint)quadVertices.Length, BufferUsage.VertexBuffer));
        _globalIndexBuffer = _resourceFactory.CreateBuffer(new BufferDescription(sizeof(ushort) * (uint)quadIndices.Length, BufferUsage.IndexBuffer));

        _graphicsDevice.UpdateBuffer(_globalVertexBuffer, 0, quadVertices);
        _graphicsDevice.UpdateBuffer(_globalIndexBuffer, 0, quadIndices);

        // vertex element descriptions define the "layout(location = x)" inputs in the shaders
        VertexLayoutDescription vertexLayoutDescription = new VertexLayoutDescription(new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2), new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4));

        var assembly = typeof(VeldridRenderer).Assembly;

        // Vertex
        using var vertStream = assembly.GetManifestResourceStream("Principle.Renderer.Shaders.base_vertex.glsl") ?? throw new FileNotFoundException();
        using var vertMs = new MemoryStream();
        vertStream.CopyTo(vertMs);
        byte[] vertexShaderBytes = vertMs.ToArray();

        // Fragment
        using var fragStream = assembly.GetManifestResourceStream("Principle.Renderer.Shaders.base_fragment.glsl") ?? throw new FileNotFoundException();
        using var fragMs = new MemoryStream();
        fragStream.CopyTo(fragMs);
        byte[] fragmentShaderBytes = fragMs.ToArray();


        ShaderDescription vertexShaderDescription = new ShaderDescription(ShaderStages.Vertex, vertexShaderBytes, "main");
        ShaderDescription fragmentShaderDescription = new ShaderDescription(ShaderStages.Fragment, fragmentShaderBytes, "main");

        _shaderResources = _resourceFactory.CreateFromSpirv(vertexShaderDescription, fragmentShaderDescription);

        GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
        pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;
        pipelineDescription.DepthStencilState = new DepthStencilStateDescription(depthTestEnabled: true, depthWriteEnabled: true, comparisonKind: ComparisonKind.LessEqual);
        pipelineDescription.RasterizerState = new RasterizerStateDescription(cullMode: FaceCullMode.Back, fillMode: PolygonFillMode.Solid, frontFace: FrontFace.Clockwise, depthClipEnabled: true, scissorTestEnabled: false);
        pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
        pipelineDescription.ResourceLayouts = System.Array.Empty<ResourceLayout>();
        pipelineDescription.ShaderSet = new ShaderSetDescription(vertexLayouts: new VertexLayoutDescription[] { vertexLayoutDescription }, shaders: _shaderResources);
        pipelineDescription.Outputs = _graphicsDevice.SwapchainFramebuffer.OutputDescription;
        _graphicsPipeline = _resourceFactory.CreateGraphicsPipeline(pipelineDescription);

        _commandList = _resourceFactory.CreateCommandList();
    }

    public void PrepareFrame()
    {
        if (Initialized == false)
        {
            throw new RendererNotInitializedException("Renderer is not initialized! Please ensure you call `Initialize(IRenderSurface)` before issuing render commands.");
        }

        _commandList!.Begin();
        _commandList!.SetFramebuffer(_graphicsDevice!.SwapchainFramebuffer);
    }

    public void Render()
    {
        if (Initialized == false)
        {
            throw new RendererNotInitializedException("Renderer is not initialized! Please ensure you call `Initialize(IRenderSurface)` before issuing render commands.");
        }

        ClearColor();

        // ABSTRACT AWAY
        _commandList!.SetVertexBuffer(0, _globalVertexBuffer);
        _commandList!.SetIndexBuffer(_globalIndexBuffer, IndexFormat.UInt16);
        _commandList!.SetPipeline(_graphicsPipeline);
        _commandList!.DrawIndexed(indexCount: 4, instanceCount: 1, indexStart: 0, vertexOffset: 0, instanceStart: 0);
    }

    public void FinalizeFrame()
    {
        if (Initialized == false)
        {
            throw new RendererNotInitializedException("Renderer is not initialized! Please ensure you call `Initialize(IRenderSurface)` before issuing render commands.");
        }

        _commandList!.End();

        _graphicsDevice!.SubmitCommands(_commandList!);
        _graphicsDevice!.SwapBuffers();
    }

    public void ClearColor()
    {
        if (Initialized == false)
        {
            throw new RendererNotInitializedException("Renderer is not initialized! Please ensure you call `Initialize(IRenderSurface)` before issuing render commands.");
        }

        _commandList!.ClearColorTarget(0, _clearColor);
    }

    public void SetClearColor(Color clearColor)
    {
        FrameClearColor = clearColor;
    }

    public void Dispose()
    {
        _graphicsDevice?.WaitForIdle();
        _graphicsDevice?.Dispose();
    }
}