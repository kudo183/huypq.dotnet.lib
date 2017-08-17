using System;

namespace huypq.Logging
{
    public interface ILoggerProcessor : IDisposable
    {
        void EnqueueMessage(string message);
    }
}
