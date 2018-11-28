using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace huypq.Logging
{
    public interface ILogBatchWriter
    {
        Task Write(List<LogEntry> logEntries);
    }

    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
    }
}
