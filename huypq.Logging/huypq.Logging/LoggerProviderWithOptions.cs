using System;
using Microsoft.Extensions.Logging;

namespace huypq.Logging
{
    public class LoggerProviderWithOptions : LoggerProvider
    {
        public class Options
        {
            public Func<string, LogLevel, bool> Filter { get; set; }
            public bool IsIncludeScope { get; set; }
            public ILoggerProcessor Processor { get; set; }
        }

        public LoggerProviderWithOptions(Func<string, LogLevel, bool> filter, bool isIncludeScope, ILoggerProcessor processor)
            : base(filter, isIncludeScope, processor)
        {
        }

        public LoggerProviderWithOptions(Options options)
            : this(options.Filter, options.IsIncludeScope, options.Processor)
        {

        }
    }
}
