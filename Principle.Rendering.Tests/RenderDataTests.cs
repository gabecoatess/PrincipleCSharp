using System.Collections.Immutable;
using Principle.Rendering.Abstractions;

namespace Principle.Rendering.Tests;

public sealed class RenderDataTests
{
    [Fact]
    public void RenderFrameCreateCopiesTheCommandArray()
    {
        RenderCommand[] source = [new ClearTargetCommand(new RenderColor(1, 2, 3))];

        var frame = RenderFrame.Create(source);
        source[0] = new ClearTargetCommand(new RenderColor(9, 9, 9));

        var command = Assert.IsType<ClearTargetCommand>(Assert.Single(frame.Commands));
        Assert.Equal(new RenderColor(1, 2, 3), command.Color);
    }

    [Fact]
    public void CommandsRemainInSubmissionOrder()
    {
        var clear = new ClearTargetCommand(new RenderColor(1, 2, 3));
        var rectangle = new DrawRectangleCommand(
            new RenderRectangle(4, 5, 6, 7),
            new RenderColor(8, 9, 10));

        var frame = RenderFrame.Create(clear, rectangle);

        Assert.Equal([clear, rectangle], frame.Commands);
    }

    [Fact]
    public void DefaultHandleIsInvalid()
    {
        Assert.False(default(RenderTargetHandle).IsValid);
        Assert.True(new RenderTargetHandle(1).IsValid);
    }

    [Fact]
    public void RenderImageUsesTopLeftRowMajorPixelAccess()
    {
        var pixels = ImmutableArray.Create(
            new RenderColor(1, 0, 0),
            new RenderColor(2, 0, 0),
            new RenderColor(3, 0, 0),
            new RenderColor(4, 0, 0));
        var image = new RenderImage(2, 2, pixels);

        Assert.Equal(pixels[0], image.GetPixel(0, 0));
        Assert.Equal(pixels[3], image.GetPixel(1, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => image.GetPixel(2, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => image.GetPixel(0, -1));
    }

    [Fact]
    public void RenderImageRejectsMismatchedDimensions()
    {
        Assert.Throws<ArgumentException>(
            () => new RenderImage(2, 2, ImmutableArray.Create(new RenderColor())));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new RenderImage(0, 2, ImmutableArray<RenderColor>.Empty));
    }

    [Fact]
    public void FailedResultDoesNotExposeAValue()
    {
        var result = RenderResult<int>.Failure(RenderErrorCode.InvalidState, "failed");

        Assert.False(result.IsSuccess);
        Assert.Equal(RenderErrorCode.InvalidState, result.Error!.Code);
        Assert.Throws<InvalidOperationException>(() => result.Value);
    }
}
