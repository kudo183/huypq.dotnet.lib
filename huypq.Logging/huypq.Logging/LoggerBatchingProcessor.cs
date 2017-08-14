using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huypq.Logging
{
    public class LoggerBatchingProcessor : ILoggerProcessor
    {
        class LogEntry
        {
            public DateTime Timestamp { get; set; }
            public string Message { get; set; }
        }

        private string _path = "";
        private string _fileName = "log";
        private int _maxRetainedFiles = 31;
        private int _maxFileSize = 200 * 1024 * 1024;

        private readonly List<LogEntry> _currentBatch = new List<LogEntry>();
        private readonly int _interval = 1000;
        private readonly int _batchSize = 1024;

        private readonly int _queueSize = 1024;
        private readonly BlockingCollection<LogEntry> _messageQueue;
        private readonly Task _outputTask;

        public LoggerBatchingProcessor(int processInterval, int batchSize, int queueSize, string path, int maxRetainedFiles, int maxFileSize)
        {
            _path = path;
            _maxRetainedFiles = maxRetainedFiles;
            _maxFileSize = maxFileSize;

            _interval = processInterval;
            _batchSize = batchSize;
            _queueSize = queueSize;

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

        System.Net.Sockets.UdpClient client = new System.Net.Sockets.UdpClient();
        private async Task WriteMessage(List<LogEntry> logEntries)
        {
            Directory.CreateDirectory(_path);

            foreach (var group in logEntries.GroupBy(GetGrouping))
            {
                var fullName = GetFullName(group.Key);
                var fileInfo = new FileInfo(fullName);
                if (_maxFileSize > 0 && fileInfo.Exists && fileInfo.Length > _maxFileSize)
                {
                    return;
                }

                using (var streamWriter = File.AppendText(fullName))
                {
                    byte[] sendBytes;
                    foreach (var item in group)
                    {
                        await streamWriter.WriteAsync(item.Message);
                        sendBytes = Encoding.ASCII.GetBytes(item.Message);
                        await client.SendAsync(sendBytes, sendBytes.Length, "localhost", 11000);
                    }
                }
            }

            RollFiles();
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

        private string GetFullName(string group)
        {
            return System.IO.Path.Combine(_path, $"{_fileName}{group}.txt");
        }

        string GetGrouping(LogEntry message)
        {
            return $"{message.Timestamp.Year:0000}{message.Timestamp.Month:00}{message.Timestamp.Day:00}";
        }

        protected void RollFiles()
        {
            if (_maxRetainedFiles > 0)
            {
                var files = new DirectoryInfo(_path)
                    .GetFiles(_fileName + "*")
                    .OrderByDescending(f => f.Name)
                    .Skip(_maxRetainedFiles);

                foreach (var item in files)
                {
                    item.Delete();
                }
            }
        }
    }
}
