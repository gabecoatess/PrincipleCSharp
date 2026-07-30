namespace Principle.Rendering.Abstractions;

public enum RenderErrorCode
{
    InvalidFrame,
    InvalidHandle,
    UnsupportedTargetOperation,
    ResourceCreationFailed,
    ResourceDestructionFailed,
    ReadbackFailed,
    ImageExportFailed,
    WrongThread,
    InvalidState,
    Disposed,
    WindowCreationFailed,
    BackendFailure
}

public sealed record RenderError(RenderErrorCode Code, string Message, string? Detail = null);

public readonly struct RenderResult
{
    private RenderResult(RenderError? error)
    {
        Error = error;
    }

    public bool IsSuccess => Error is null;

    public RenderError? Error { get; }

    public static RenderResult Success() => new(null);

    public static RenderResult Failure(RenderErrorCode code, string message, string? detail = null) =>
        new(new RenderError(code, message, detail));
}

public readonly struct RenderResult<T>
{
    private readonly T? _value;

    private RenderResult(T value)
    {
        _value = value;
        Error = null;
    }

    private RenderResult(RenderError error)
    {
        _value = default;
        Error = error;
    }

    public bool IsSuccess => Error is null;

    public RenderError? Error { get; }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("A failed render result does not contain a value.");

    public static RenderResult<T> Success(T value) => new(value);

    public static RenderResult<T> Failure(
        RenderErrorCode code,
        string message,
        string? detail = null) =>
        new(new RenderError(code, message, detail));
}
