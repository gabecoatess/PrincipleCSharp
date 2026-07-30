using Principle.Rendering.Abstractions;
using Principle.Rendering.Raylib;

namespace Principle.Rendering.Tests;

public sealed class RaylibInternalTests
{
    [Fact]
    public void CommandValidatorRejectsInvalidFramesAndRectangles()
    {
        Assert.Equal(
            RenderErrorCode.InvalidFrame,
            RaylibCommandValidator.Validate(new RenderFrame(default)).Error!.Code);

        var invalidRectangle = RenderFrame.Create(
            new DrawRectangleCommand(
                new RenderRectangle(0, 0, -1, 2),
                new RenderColor(1, 2, 3)));

        Assert.Equal(
            RenderErrorCode.InvalidFrame,
            RaylibCommandValidator.Validate(invalidRectangle).Error!.Code);
    }

    [Fact]
    public void TargetRegistryUsesOpaqueMonotonicHandlesAndRejectsWindowRemoval()
    {
        var registry = new RaylibRenderTargetRegistry();

        var window = registry.RegisterWindow();
        var offscreen = registry.RegisterOffscreen(default);

        Assert.True(window.IsValid);
        Assert.True(offscreen.Value > window.Value);
        Assert.False(registry.TryRemoveOffscreen(window, out _));
        Assert.True(registry.TryRemoveOffscreen(offscreen, out var removed));
        Assert.Equal(RaylibRenderTargetKind.Offscreen, removed.Kind);
        Assert.False(registry.TryGet(offscreen, out _));
    }

    [Fact]
    public async Task ContextRejectsRenderingFromAnotherThread()
    {
        var registry = new RaylibRenderTargetRegistry();
        var context = new RaylibContext(registry, registry.RegisterWindow(), () => { });

        var result = await Task.Run(context.CheckRenderAccess);

        Assert.Equal(RenderErrorCode.WrongThread, result.Error!.Code);
    }
}
