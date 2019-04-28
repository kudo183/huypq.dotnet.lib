using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace huypq.Logging
{
    public class LoggerBatchingProcessor : ILoggerProcessor
    {
        private readonly List<LogEntry> _currentBatch = new List<LogEntry>();
        private readonly int _interval = 1000;
        private readonly int _batchSize = 1024;

        private readonly int _queueSize = 1024;
        private readonly BlockingCollection<LogEntry> _messageQueue;
        private readonly Task _outputTask;
        private readonly List<ILogBatchWriter> _logWriters = new List<ILogBatchWriter>();

        public LoggerBatchingProcessor(int processInterval, int batchSize, int queueSize, string path, int maxRetainedFiles, int maxFileSize) : this()
        {
            _logWriters.Add(new FileBatchWriter(path, "log_", maxRetainedFiles, maxFileSize));

            _interval = processInterval;
            _batchSize = batchSize;
            _queueSize = queueSize;
        }

        public LoggerBatchingProcessor(ILogBatchWriter logWriter = null) : this()
        {
            if (logWriter != null)
            {
                _logWriters.Add(logWriter);
            }
            else
            {
                _logWriters.Add(new ConsoleBatchWriter());
            }
        }

        public LoggerBatchingProcessor(List<ILogBatchWriter> logWriter) : this()
        {
            if (logWriter != null)
            {
                _logWriters.AddRange(logWriter);
            }
            else
            {
                _logWriters.Add(new ConsoleBatchWriter());
            }
        }

        LoggerBatchingProcessor()
        {
            _messageQueue = new BlockingCollection<LogEntry>(_queueSize);

            // Start message queue processor
            _outputTask = Task.Factory.StartNew(
                ProcessLogQueue,
                this,
                TaskCreationOptions.LongRunning);
        }

        public void EnqueueMessage(string message)
        {
            if (!_messageQueue.IsAddingCompleted)
            {
                try
                {
                    _messageQueue.Add(new LogEntry() { Timestamp = DateTime.UtcNow, Message = message });
                    return;
                }
                catch (InvalidOperationException) { }
            }
        }

        private async Task WriteMessage(List<LogEntry> logEntries)
        {
            foreach (var writer in _logWriters)
            {
                try
                {
                    await writer.Write(logEntries);
                }
                catch { }
            }
        }

        private async void ProcessLogQueue()
        {
            while (_messageQueue.IsCompleted == false)
            {
                var limit = _batchSize;

                while (limit > 0 && _messageQueue.TryTake(out var message))
                {
                    _currentBatch.Add(message);
                    limit--;
                }

                if (_currentBatch.Count > 0)
                {
                    await WriteMessage(_currentBatch);

                    _currentBatch.Clear();
                }

                await Task.Delay(_interval);
            }
        }

        private static void ProcessLogQueue(object state)
        {
            var consoleLogger = (LoggerBatchingProcessor)state;

            consoleLogger.ProcessLogQueue();
        }

        public void Dispose()
        {
            _messageQueue.CompleteAdding();

            try
            {
                _outputTask.Wait(1500); // with timeout in-case Console is locked by user input
            }
            catch (TaskCanceledException) { }
            catch (AggregateException ex) when (ex.InnerExceptions.Count == 1 && ex.InnerExceptions[0] is TaskCanceledException) { }
        }
    }

    public class ConsoleBatchWriter : ILogBatchWriter
    {
        public async Task Write(List<LogEntry> logEntries)
        {
            foreach (var msg in logEntries)
            {
                Console.WriteLine(msg.Message);
            }
        }
    }

    public class UDPBatchWriter : ILogBatchWriter
    {
        readonly System.Net.Sockets.UdpClient _udpClient = new System.Net.Sockets.UdpClient();
        readonly string _host;
        readonly int _port;
        public UDPBatchWriter(string host, int port)
        {
            _host = host;
            _port = port;
        }
        public async Task Write(List<LogEntry> logEntries)
        {
            foreach (var msg in logEntries)
            {
                byte[] sendBytes = System.Text.Encoding.ASCII.GetBytes(msg.Message);
                await _udpClient.SendAsync(sendBytes, sendBytes.Length, _host, _port);
            }
        }
    }
}
