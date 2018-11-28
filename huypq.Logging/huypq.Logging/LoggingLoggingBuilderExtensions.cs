using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace huypq.Logging
{
    public static class LoggingLoggingBuilderExtensions
    {
        public static ILoggingBuilder AddLogging(this ILoggingBuilder builder, ILoggerProcessor loggerProcessor, LogLevel logLevel)
        {
            builder.AddProvider(new LoggerProvider(
                (category, level) => true,
                true,
                loggerProcessor));

            builder.AddFilter<LoggerProvider>(null, logLevel);

            return builder;
        }

        public static ILoggingBuilder AddLogging(this ILoggingBuilder builder, ILoggerProcessor loggerProcessor, List<Tuple<string, LogLevel>> logLevels)
        {
            builder.AddProvider(new LoggerProvider(
                (category, level) => true,
                true,
                loggerProcessor));

            foreach (var item in logLevels)
            {
                builder.AddFilter<LoggerProvider>(item.Item1, item.Item2);
            }

            return builder;
        }
    }
}
