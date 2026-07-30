using Principle.Rendering.Abstractions;
using Raylib_cs;

namespace Principle.Rendering.Raylib;

internal enum RaylibRenderTargetKind
{
    Window,
    Offscreen
}

internal sealed record RaylibRenderTargetEntry(
    RaylibRenderTargetKind Kind,
    RenderTexture2D RenderTexture);

internal sealed class RaylibRenderTargetRegistry
{
    private readonly Dictionary<RenderTargetHandle, RaylibRenderTargetEntry> _targets = [];
    private ulong _nextHandle = 1;

    public RenderTargetHandle RegisterWindow()
    {
        var handle = AllocateHandle();
        _targets.Add(handle, new RaylibRenderTargetEntry(RaylibRenderTargetKind.Window, default));
        return handle;
    }

    public RenderTargetHandle RegisterOffscreen(RenderTexture2D renderTexture)
    {
        var handle = AllocateHandle();
        _targets.Add(handle, new RaylibRenderTargetEntry(RaylibRenderTargetKind.Offscreen, renderTexture));
        return handle;
    }

    public bool TryGet(RenderTargetHandle handle, out RaylibRenderTargetEntry entry) =>
        _targets.TryGetValue(handle, out entry!);

    public bool TryRemoveOffscreen(RenderTargetHandle handle, out RaylibRenderTargetEntry entry)
    {
        if (!_targets.TryGetValue(handle, out entry!) ||
            entry.Kind != RaylibRenderTargetKind.Offscreen)
        {
            return false;
        }

        return _targets.Remove(handle);
    }

    public IReadOnlyList<RaylibRenderTargetEntry> RemoveAllOffscreen()
    {
        var removed = _targets
            .Where(pair => pair.Value.Kind == RaylibRenderTargetKind.Offscreen)
            .Select(pair => pair.Value)
            .ToArray();

        var handles = _targets
            .Where(pair => pair.Value.Kind == RaylibRenderTargetKind.Offscreen)
            .Select(pair => pair.Key)
            .ToArray();

        foreach (var handle in handles)
        {
            _targets.Remove(handle);
        }

        return removed;
    }

    private RenderTargetHandle AllocateHandle()
    {
        if (_nextHandle == 0)
        {
            throw new InvalidOperationException("The render-target handle space was exhausted.");
        }

        return new RenderTargetHandle(_nextHandle++);
    }
}
