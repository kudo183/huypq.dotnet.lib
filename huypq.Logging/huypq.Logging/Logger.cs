using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace huypq.Logging
{
    public class Logger : ILogger
    {
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly ILoggerProcessor _messageQueue;
        private readonly string _name;
        private readonly bool _isIncludeScope;

        public Logger(string name, Func<string, LogLevel, bool> filter, bool isIncludeScope, ILoggerProcessor messageQueue)
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
                WriteJsonMessage(logLevel, _name, eventId.Id, message, exception, state as IEnumerable<KeyValuePair<string, object>>);
            }
        }

        private void WriteJsonMessage(LogLevel logLevel, string logName, int eventId, string message, Exception exception, IEnumerable<KeyValuePair<string, object>> state)
        {
            using (var sw = new StringWriter())
            using (var jtw = new JsonTextWriter(sw))
            {
                jtw.WriteStartObject();
                jtw.WritePropertyName("time");
                jtw.WriteValue(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
                jtw.WritePropertyName("level");
                jtw.WriteValue(GetLogLevelString(logLevel));
                jtw.WritePropertyName("cat");
                jtw.WriteValue(logName);
                jtw.WritePropertyName("eventID");
                jtw.WriteValue(eventId);
                if (_isIncludeScope)
                {
                    jtw.WritePropertyName("scope");
                    jtw.WriteValue(GetScopeInformation());
                }
                if (string.IsNullOrEmpty(message) == false)
                {
                    jtw.WritePropertyName("msg");
                    jtw.WriteValue(message);
                }
                if (exception != null)
                {
                    jtw.WritePropertyName("ex");
                    jtw.WriteValue(exception.ToString());
                }
                if (state != null)
                {
                    foreach (var item in state)
                    {
                        if (item.Key == "{OriginalFormat}")
                            continue;
                        jtw.WritePropertyName(item.Key);
                        if (item.Value != null)
                        {
                            try
                            {
                                jtw.WriteValue(item.Value);
                            }
                            catch (Exception)
                            {
                                jtw.WriteValue(item.Value.ToString());
                            }
                        }
                        else
                        {
                            jtw.WriteNull();
                        }
                    }
                }
                jtw.WriteEndObject();
                sw.WriteLine();
                _messageQueue.EnqueueMessage(sw.ToString());
            }
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
