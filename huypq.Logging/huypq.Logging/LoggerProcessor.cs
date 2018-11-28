using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace huypq.Logging
{
    public class LoggerProcessor : ILoggerProcessor
    {
        private const int _maxQueuedMessages = 1024;

        private readonly BlockingCollection<string> _messageQueue = new BlockingCollection<string>(_maxQueuedMessages);
        private readonly Task _outputTask;
        private readonly List<ILogWriter> _logWriters = new List<ILogWriter>();

        public LoggerProcessor(ILogWriter logWriter = null) : this()
        {
            if (logWriter != null)
            {
                _logWriters.Add(logWriter);
            }
            else
            {
                _logWriters.Add(new ConsoleWriter());
            }
        }

        public LoggerProcessor(List<ILogWriter> logWriter) : this()
        {
            if (logWriter != null)
            {
                _logWriters.AddRange(logWriter);
            }
            else
            {
                _logWriters.Add(new ConsoleWriter());
            }

        }

        LoggerProcessor()
        {
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
                    _messageQueue.Add(message);
                    return;
                }
                catch (InvalidOperationException) { }
            }
        }

        private void WriteMessage(string message)
        {
            foreach (var writer in _logWriters)
            {
                writer.Write(message);
            }
        }

        private void ProcessLogQueue()
        {
            foreach (var message in _messageQueue.GetConsumingEnumerable())
            {
                WriteMessage(message);
            }
        }

        private static void ProcessLogQueue(object state)
        {
            var consoleLogger = (LoggerProcessor)state;

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

    public class ConsoleWriter : ILogWriter
    {
        public void Write(string msg)
        {
            Console.WriteLine(msg);
        }
    }

    public class FileWriter : ILogWriter
    {
        readonly string _filePath;
        public FileWriter(string filePath)
        {
            _filePath = filePath;
        }
        public void Write(string msg)
        {
            System.IO.File.AppendAllText(_filePath, msg);
        }
    }

    public class UDPWriter : ILogWriter
    {
        readonly System.Net.Sockets.UdpClient _udpClient = new System.Net.Sockets.UdpClient();
        readonly string _host;
        readonly int _port;
        public UDPWriter(string host, int port)
        {
            _host = host;
            _port = port;
        }
        public void Write(string msg)
        {
            byte[] sendBytes = System.Text.Encoding.ASCII.GetBytes(msg);
            _udpClient.SendAsync(sendBytes, sendBytes.Length, _host, _port);
        }
    }
}
