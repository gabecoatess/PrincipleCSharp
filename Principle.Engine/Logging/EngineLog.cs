using Serilog;

namespace Principle.Engine.Logging;

internal static class EngineLog
{
    private static readonly ILogger Log = Logging.LogCore.Instance.ForContext("LogSource", "ENGINE");

    #region Verbose
    //public static void Verbose(string messageTemplate) => Log.Verbose(messageTemplate);

    //public static void Verbose<T>(string messageTemplate, T value) => Log.Verbose(messageTemplate, value);

    //public static void Verbose<T0, T1>(string messageTemplate, T0 value0, T1 value1) =>
    //    Log.Verbose(messageTemplate, value0, value1);

    //public static void Verbose<T0, T1, T2>(string messageTemplate, T0 value0, T1 value1, T2 value2) =>
    //    Log.Verbose(messageTemplate, value0, value1, value2);

    //public static void Verbose(string message, params object?[]? propertyValues) => Log.Verbose(message, propertyValues);

    //public static void Verbose(Exception? exception, string messageTemplate) => Log.Verbose(exception, messageTemplate);

    //public static void Verbose<T>(Exception? exception, string messageTemplate, T value) =>
    //    Log.Verbose(exception, messageTemplate, value);

    //public static void Verbose<T0, T1>(Exception? exception, string messageTemplate, T0 value0, T1 value1) =>
    //    Log.Verbose(exception, messageTemplate, value0, value1);

    //public static void Verbose<T0, T1, T2>(Exception? exception, string messageTemplate, T0 value0, T1 value1, T2 value2) =>
    //    Log.Verbose(exception, messageTemplate, value0, value1, value2);

    //public static void Verbose(Exception? exception, string messageTemplate, params object?[]? propertyValues) =>
    //    Log.Verbose(exception, messageTemplate, propertyValues);
    #endregion

    #region Debug
    public static void Debug(string messageTemplate) => Log.Debug(messageTemplate);

    public static void Debug<T>(string messageTemplate, T value) => Log.Debug(messageTemplate, value);

    public static void Debug<T0, T1>(string messageTemplate, T0 value0, T1 value1) =>
        Log.Debug(messageTemplate, value0, value1);

    public static void Debug<T0, T1, T2>(string messageTemplate, T0 value0, T1 value1, T2 value2) =>
        Log.Debug(messageTemplate, value0, value1, value2);

    public static void Debug(string message, params object?[]? propertyValues) => Log.Debug(message, propertyValues);

    public static void Debug(Exception? exception, string messageTemplate) => Log.Debug(exception, messageTemplate);

    public static void Debug<T>(Exception? exception, string messageTemplate, T value) =>
        Log.Debug(exception, messageTemplate, value);

    public static void Debug<T0, T1>(Exception? exception, string messageTemplate, T0 value0, T1 value1) =>
        Log.Debug(exception, messageTemplate, value0, value1);

    public static void Debug<T0, T1, T2>(Exception? exception, string messageTemplate, T0 value0, T1 value1, T2 value2) =>
        Log.Debug(exception, messageTemplate, value0, value1, value2);

    public static void Debug(Exception? exception, string messageTemplate, params object?[]? propertyValues) =>
        Log.Debug(exception, messageTemplate, propertyValues);
    #endregion

    #region Information
    public static void Information(string messageTemplate) => Log.Information(messageTemplate);

    public static void Information<T>(string messageTemplate, T value) => Log.Information(messageTemplate, value);

    public static void Information<T0, T1>(string messageTemplate, T0 value0, T1 value1) =>
        Log.Information(messageTemplate, value0, value1);

    public static void Information<T0, T1, T2>(string messageTemplate, T0 value0, T1 value1, T2 value2) =>
        Log.Information(messageTemplate, value0, value1, value2);

    public static void Information(string message, params object?[]? propertyValues) => Log.Information(message, propertyValues);

    public static void Information(Exception? exception, string messageTemplate) => Log.Information(exception, messageTemplate);

    public static void Information<T>(Exception? exception, string messageTemplate, T value) =>
        Log.Information(exception, messageTemplate, value);

    public static void Information<T0, T1>(Exception? exception, string messageTemplate, T0 value0, T1 value1) =>
        Log.Information(exception, messageTemplate, value0, value1);

    public static void Information<T0, T1, T2>(Exception? exception, string messageTemplate, T0 value0, T1 value1, T2 value2) =>
        Log.Information(exception, messageTemplate, value0, value1, value2);

    public static void Information(Exception? exception, string messageTemplate, params object?[]? propertyValues) =>
        Log.Information(exception, messageTemplate, propertyValues);
    #endregion

    #region Warning
    public static void Warning(string messageTemplate) => Log.Warning(messageTemplate);

    public static void Warning<T>(string messageTemplate, T value) => Log.Warning(messageTemplate, value);

    public static void Warning<T0, T1>(string messageTemplate, T0 value0, T1 value1) =>
        Log.Warning(messageTemplate, value0, value1);

    public static void Warning<T0, T1, T2>(string messageTemplate, T0 value0, T1 value1, T2 value2) =>
        Log.Warning(messageTemplate, value0, value1, value2);

    public static void Warning(string message, params object?[]? propertyValues) => Log.Warning(message, propertyValues);

    public static void Warning(Exception? exception, string messageTemplate) => Log.Warning(exception, messageTemplate);

    public static void Warning<T>(Exception? exception, string messageTemplate, T value) =>
        Log.Warning(exception, messageTemplate, value);

    public static void Warning<T0, T1>(Exception? exception, string messageTemplate, T0 value0, T1 value1) =>
        Log.Warning(exception, messageTemplate, value0, value1);

    public static void Warning<T0, T1, T2>(Exception? exception, string messageTemplate, T0 value0, T1 value1, T2 value2) =>
        Log.Warning(exception, messageTemplate, value0, value1, value2);

    public static void Warning(Exception? exception, string messageTemplate, params object?[]? propertyValues) =>
        Log.Warning(exception, messageTemplate, propertyValues);
    #endregion

    #region Error
    public static void Error(string messageTemplate) => Log.Error(messageTemplate);

    public static void Error<T>(string messageTemplate, T value) => Log.Error(messageTemplate, value);

    public static void Error<T0, T1>(string messageTemplate, T0 value0, T1 value1) =>
        Log.Error(messageTemplate, value0, value1);

    public static void Error<T0, T1, T2>(string messageTemplate, T0 value0, T1 value1, T2 value2) =>
        Log.Error(messageTemplate, value0, value1, value2);

    public static void Error(string message, params object?[]? propertyValues) => Log.Error(message, propertyValues);

    public static void Error(Exception? exception, string messageTemplate) => Log.Error(exception, messageTemplate);

    public static void Error<T>(Exception? exception, string messageTemplate, T value) =>
        Log.Error(exception, messageTemplate, value);

    public static void Error<T0, T1>(Exception? exception, string messageTemplate, T0 value0, T1 value1) =>
        Log.Error(exception, messageTemplate, value0, value1);

    public static void Error<T0, T1, T2>(Exception? exception, string messageTemplate, T0 value0, T1 value1, T2 value2) =>
        Log.Error(exception, messageTemplate, value0, value1, value2);

    public static void Error(Exception? exception, string messageTemplate, params object?[]? propertyValues) =>
        Log.Error(exception, messageTemplate, propertyValues);
    #endregion

    #region Fatal
    public static void Fatal(string messageTemplate) => Log.Fatal(messageTemplate);

    public static void Fatal<T>(string messageTemplate, T value) => Log.Fatal(messageTemplate, value);

    public static void Fatal<T0, T1>(string messageTemplate, T0 value0, T1 value1) =>
        Log.Fatal(messageTemplate, value0, value1);

    public static void Fatal<T0, T1, T2>(string messageTemplate, T0 value0, T1 value1, T2 value2) =>
        Log.Fatal(messageTemplate, value0, value1, value2);

    public static void Fatal(string message, params object?[]? propertyValues) => Log.Fatal(message, propertyValues);

    public static void Fatal(Exception? exception, string messageTemplate) => Log.Fatal(exception, messageTemplate);

    public static void Fatal<T>(Exception? exception, string messageTemplate, T value) =>
        Log.Fatal(exception, messageTemplate, value);

    public static void Fatal<T0, T1>(Exception? exception, string messageTemplate, T0 value0, T1 value1) =>
        Log.Fatal(exception, messageTemplate, value0, value1);

    public static void Fatal<T0, T1, T2>(Exception? exception, string messageTemplate, T0 value0, T1 value1, T2 value2) =>
        Log.Fatal(exception, messageTemplate, value0, value1, value2);

    public static void Fatal(Exception? exception, string messageTemplate, params object?[]? propertyValues) =>
        Log.Fatal(exception, messageTemplate, propertyValues);
    #endregion
}
