using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace Server.Logger
{
    public class LoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, Logger> _loggers = new ConcurrentDictionary<string, Logger>();
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly bool _isIncludeScope;
        private readonly LoggerProcessor _messageQueue = new LoggerProcessor();

        public LoggerProvider(Func<string, LogLevel, bool> filter, bool isIncludeScope)
        {
            _filter = filter;
            _isIncludeScope = isIncludeScope;
        }

        public ILogger CreateLogger(string name)
        {
            return _loggers.GetOrAdd(name, CreateLoggerImplementation);
        }

        private Logger CreateLoggerImplementation(string name)
        {
            return new Logger(name, _filter, _isIncludeScope, _messageQueue);
        }

        public void Dispose()
        {
            _messageQueue.Dispose();
        }
    }
}
