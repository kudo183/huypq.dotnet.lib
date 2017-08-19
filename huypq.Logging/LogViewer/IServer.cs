using System;
using System.Threading.Tasks;

namespace LogViewer
{
    public interface IServer
    {
        bool IsRunning { get; }
        Action<string> ReadCompleted { get; set; }
        Action Started { get; set; }
        Action Stopped { get; set; }
        Task Start();
        void Stop();
    }
}
