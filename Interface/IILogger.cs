using ServerBlockChain.Entities.Enum;

namespace ServerBlockChain.Interface;

/// <summary>
/// Defines a contract for logging operations with generic type support.
/// Handles different types of log entries with various levels of severity.
/// </summary>
/// <typeparam name="T">The type of data being logged</typeparam>
public interface IILogger<in T>
{
    /// <summary>
    /// Logs data with exception details and specified severity level.
    /// </summary>
    /// <param name="data">The generic data to be logged</param>
    /// <param name="exception">The exception associated with the log entry</param>
    /// <param name="message">The log message describing the event</param>
    /// <param name="level">The severity level of the log entry</param>
    void Log(T data, Exception exception, string message, LogLevel level);

    /// <summary>
    /// Logs data with specified severity level.
    /// </summary>
    /// <param name="data">The generic data to be logged</param>
    /// <param name="message">The log message describing the event</param>
    /// <param name="level">The severity level of the log entry</param>
    void Log(T data, string message, LogLevel level);

    /// <summary>
    /// Logs non-generic object data with exception details and specified severity level.
    /// </summary>
    /// <param name="data">The object data to be logged</param>
    /// <param name="exception">The exception associated with the log entry</param>
    /// <param name="message">The log message describing the event</param>
    /// <param name="level">The severity level of the log entry</param>
    void Log(object data, Exception exception, string message, LogLevel level);

    /// <summary>
    /// Logs non-generic object data with specified severity level.
    /// </summary>
    /// <param name="data">The object data to be logged</param>
    /// <param name="message">The log message describing the event</param>
    /// <param name="level">The severity level of the log entry</param>
    void Log(object data, string message, LogLevel level);
}