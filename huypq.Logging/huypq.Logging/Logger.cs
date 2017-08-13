using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace Server.Logger
{
    public class Logger : ILogger
    {
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly LoggerProcessor _messageQueue;
        private readonly string _name;
        private readonly bool _isIncludeScope;

        public Logger(string name, Func<string, LogLevel, bool> filter, bool isIncludeScope, LoggerProcessor messageQueue)
        {
            _name = name;
            _filter = filter;
            _isIncludeScope = isIncludeScope;
            _messageQueue = messageQueue;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            return LogScope.Push(_name, state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _filter(_name, logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message) == false || exception != null)
            {
                WriteJsonMessage(logLevel, _name, eventId.Id, message, exception);
            }
        }

        private void WriteJsonMessage(LogLevel logLevel, string logName, int eventId, string message, Exception exception)
        {
            var sw = new StringWriter();
            var jtw = new JsonTextWriter(sw);

            jtw.WriteStartObject();
            jtw.WritePropertyName("t");
            jtw.WriteValue(DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:ss.fffZ"));
            jtw.WritePropertyName("a");
            jtw.WriteValue(GetLogLevelString(logLevel));
            jtw.WritePropertyName("b");
            jtw.WriteValue(logName);
            jtw.WritePropertyName("c");
            jtw.WriteValue(eventId);
            if (_isIncludeScope)
            {
                jtw.WritePropertyName("d");
                jtw.WriteValue(GetScopeInformation());
            }
            if (string.IsNullOrEmpty(message) == false)
            {
                jtw.WritePropertyName("e");
                jtw.WriteValue(message);
            }
            if (exception != null)
            {
                jtw.WritePropertyName("f");
                jtw.WriteValue(exception.ToString());
            }
            jtw.WriteEndObject();
            sw.WriteLine();
            _messageQueue.EnqueueMessage(sw.ToString());
        }

        private string GetScopeInformation()
        {
            var current = LogScope.Current;
            string scopeLog = string.Empty;
            var builder = new StringBuilder();

            while (current != null)
            {
                if (builder.Length == 0)
                {
                    scopeLog = $"=> {current}";
                }
                else
                {
                    scopeLog = $"=> {current} ";
                }

                builder.Insert(0, scopeLog);
                current = current.Parent;
            }

            return builder.ToString();
        }

        private static string GetLogLevelString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return "trce";
                case LogLevel.Debug:
                    return "dbug";
                case LogLevel.Information:
                    return "info";
                case LogLevel.Warning:
                    return "warn";
                case LogLevel.Error:
                    return "fail";
                case LogLevel.Critical:
                    return "crit";
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }
    }
}
