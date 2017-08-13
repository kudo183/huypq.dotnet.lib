using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
namespace Server.Logger
{
    public class LoggerProcessor : IDisposable
    {
        private const int _maxQueuedMessages = 1024;

        private readonly BlockingCollection<string> _messageQueue = new BlockingCollection<string>(_maxQueuedMessages);
        private readonly Task _outputTask;

        public LoggerProcessor()
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

        System.Net.Sockets.UdpClient client = new System.Net.Sockets.UdpClient();
        private void WriteMessage(string message)
        {
            System.IO.File.AppendAllText(@"c:\log.txt", message);
            byte[] sendBytes = System.Text.Encoding.ASCII.GetBytes(message);
            client.SendAsync(sendBytes, sendBytes.Length, "localhost", 11000);
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
}
