namespace Principle.Platform;

public readonly record struct WindowDescription(
    string Title,
    int Width,
    int Height,
    bool IsResizable = true,
    bool IsVisible = true);

public readonly record struct WindowState(
    int LogicalWidth,
    int LogicalHeight,
    int DrawableWidth,
    int DrawableHeight,
    bool WasResized,
    bool CloseRequested);

public interface IPlatformWindow
{
    PlatformResult<WindowState> PollEvents();

    PlatformResult Close();
}

public enum PlatformErrorCode
{
    WrongThread,
    InvalidState,
    Disposed,
    NativeFailure
}

public sealed record PlatformError(PlatformErrorCode Code, string Message, string? Detail = null);

public readonly struct PlatformResult
{
    private PlatformResult(PlatformError? error)
    {
        Error = error;
    }

    public bool IsSuccess => Error is null;

    public PlatformError? Error { get; }

    public static PlatformResult Success() => new(null);

    public static PlatformResult Failure(PlatformErrorCode code, string message, string? detail = null) =>
        new(new PlatformError(code, message, detail));
}

public readonly struct PlatformResult<T>
{
    private readonly T? _value;

    private PlatformResult(T value)
    {
        _value = value;
        Error = null;
    }

    private PlatformResult(PlatformError error)
    {
        _value = default;
        Error = error;
    }

    public bool IsSuccess => Error is null;

    public PlatformError? Error { get; }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("A failed platform result does not contain a value.");

    public static PlatformResult<T> Success(T value) => new(value);

    public static PlatformResult<T> Failure(
        PlatformErrorCode code,
        string message,
        string? detail = null) =>
        new(new PlatformError(code, message, detail));
}
