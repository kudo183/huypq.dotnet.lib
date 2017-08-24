using System;

namespace Microsoft.Extensions.Logging
{
    public static class LoggerProviderExtensions
    {
        public static ILogger CreateLogger<T>(this ILoggerProvider loggerProvider)
        {
            if (loggerProvider == null)
            {
                throw new ArgumentNullException(nameof(loggerProvider));
            }

            return loggerProvider.CreateLogger(typeof(T).FullName);
        }
    }

    public static class LoggerExtensions
    {
        private static readonly Func<object, Exception, string> _messageFormatter = MessageFormatter;
        private static string MessageFormatter(object state, Exception error)
        {
            return state.ToString();
        }

        public static void LogDebug(this ILogger logger, EventId eventId, Exception exception, string message, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(LogLevel.Debug, eventId, string.Format(message, args), exception, _messageFormatter);
        }

        public static void LogDebug(this ILogger logger, EventId eventId, string message, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(LogLevel.Debug, eventId, string.Format(message, args), null, _messageFormatter);
        }

        public static void LogDebug(this ILogger logger, Exception exception, string message, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(LogLevel.Debug, 0, string.Format(message, args), exception, _messageFormatter);
        }

        public static void LogDebug(this ILogger logger, string message, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(LogLevel.Debug, 0, string.Format(message, args), null, _messageFormatter);
        }

        public static void LogTrace(this ILogger logger, EventId eventId, Exception exception, string message, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(LogLevel.Trace, eventId, string.Format(message, args), exception, _messageFormatter);
        }

        public static void LogTrace(this ILogger logger, EventId eventId, string message, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(LogLevel.Trace, eventId, string.Format(message, args), null, _messageFormatter);
        }

        public static void LogTrace(this ILogger logger, Exception exception, string message, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(LogLevel.Trace, 0, string.Format(message, args), exception, _messageFormatter);
        }

        public static void LogTrace(this ILogger logger, string message, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(LogLevel.Trace, 0, string.Format(message, args), null, _messageFormatter);
        }

        public static void LogInformation(this ILogger logger, EventId eventId, Exception exception, string message, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(LogLevel.Information, eventId, string.Format(message, args), exception, _messageFormatter);
        }

        public static void LogInformation(this ILogger logger, EventId eventId, string message, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(LogLevel.Information, eventId, string.Format(message, args), null, _messageFormatter);
        }

        public static void LogInformation(this ILogger logger, Exception exception, string message, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(LogLevel.Information, 0, string.Format(message, args), exception, _messageFormatter);
        }

        public static void LogInformation(this ILogger logger, string message, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(LogLevel.Information, 0, string.Format(message, args), null, _messageFormatter);
        }

        public static void LogWarning(this ILogger logger, EventId eventId, Exception exception, string message, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(LogLevel.Warning, eventId, string.Format(message, args), exception, _messageFormatter);
        }

        public static void LogWarning(this ILogger logger, EventId eventId, string message, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(LogLevel.Warning, eventId, string.Format(message, args), null, _messageFormatter);
        }

        public static void LogWarning(this ILogger logger, Exception exception, string message, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(LogLevel.Warning, 0, string.Format(message, args), exception, _messageFormatter);
        }

        public static void LogWarning(this ILogger logger, string message, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(LogLevel.Warning, 0, string.Format(message, args), null, _messageFormatter);
        }

        public static void LogError(this ILogger logger, EventId eventId, Exception exception, string message, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(LogLevel.Error, eventId, string.Format(message, args), exception, _messageFormatter);
        }

        public static void LogError(this ILogger logger, EventId eventId, string message, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(LogLevel.Error, eventId, string.Format(message, args), null, _messageFormatter);
        }

        public static void LogError(this ILogger logger, Exception exception, string message, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(LogLevel.Error, 0, string.Format(message, args), exception, _messageFormatter);
        }

        public static void LogError(this ILogger logger, string message, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(LogLevel.Error, 0, string.Format(message, args), null, _messageFormatter);
        }

        public static void LogCritical(this ILogger logger, EventId eventId, Exception exception, string message, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(LogLevel.Critical, eventId, string.Format(message, args), exception, _messageFormatter);
        }

        public static void LogCritical(this ILogger logger, EventId eventId, string message, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(LogLevel.Critical, eventId, string.Format(message, args), null, _messageFormatter);
        }

        public static void LogCritical(this ILogger logger, Exception exception, string message, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(LogLevel.Critical, 0, string.Format(message, args), exception, _messageFormatter);
        }

        public static void LogCritical(this ILogger logger, string message, params object[] args)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.Log(LogLevel.Critical, 0, string.Format(message, args), null, _messageFormatter);
        }
    }

    public interface ILoggerProvider
    {
        //
        // Summary:
        //     Creates a new Microsoft.Extensions.Logging.ILogger instance.
        //
        // Parameters:
        //   categoryName:
        //     The category name for messages produced by the logger.
        ILogger CreateLogger(string categoryName);
    }

    public interface ILogger
    {
        //
        // Summary:
        //     Begins a logical operation scope.
        //
        // Parameters:
        //   state:
        //     The identifier for the scope.
        //
        // Returns:
        //     An IDisposable that ends the logical operation scope on dispose.
        IDisposable BeginScope<TState>(TState state);
        //
        // Summary:
        //     Checks if the given logLevel is enabled.
        //
        // Parameters:
        //   logLevel:
        //     level to be checked.
        //
        // Returns:
        //     true if enabled.
        bool IsEnabled(LogLevel logLevel);
        //
        // Summary:
        //     Writes a log entry.
        //
        // Parameters:
        //   logLevel:
        //     Entry will be written on this level.
        //
        //   eventId:
        //     Id of the event.
        //
        //   state:
        //     The entry to be written. Can be also an object.
        //
        //   exception:
        //     The exception related to this entry.
        //
        //   formatter:
        //     Function to create a string message of the state and exception.
        void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter);
    }

    public struct EventId
    {
        private int _id;
        private string _name;

        public EventId(int id, string name = null)
        {
            _id = id;
            _name = name;
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public static implicit operator EventId(int i)
        {
            return new EventId(i);
        }

        public override string ToString()
        {
            if (_name != null)
            {
                return _name;
            }
            else
            {
                return _id.ToString();
            }
        }
    }

    //
    // Summary:
    //     Defines logging severity levels.
    public enum LogLevel
    {
        //
        // Summary:
        //     Logs that contain the most detailed messages. These messages may contain sensitive
        //     application data. These messages are disabled by default and should never be
        //     enabled in a production environment.
        Trace = 0,
        //
        // Summary:
        //     Logs that are used for interactive investigation during development. These logs
        //     should primarily contain information useful for debugging and have no long-term
        //     value.
        Debug = 1,
        //
        // Summary:
        //     Logs that track the general flow of the application. These logs should have long-term
        //     value.
        Information = 2,
        //
        // Summary:
        //     Logs that highlight an abnormal or unexpected event in the application flow,
        //     but do not otherwise cause the application execution to stop.
        Warning = 3,
        //
        // Summary:
        //     Logs that highlight when the current flow of execution is stopped due to a failure.
        //     These should indicate a failure in the current activity, not an application-wide
        //     failure.
        Error = 4,
        //
        // Summary:
        //     Logs that describe an unrecoverable application or system crash, or a catastrophic
        //     failure that requires immediate attention.
        Critical = 5,
        //
        // Summary:
        //     Not used for writing log messages. Specifies that a logging category should not
        //     write any messages.
        None = 6
    }
}
