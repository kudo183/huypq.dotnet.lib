using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogViewer
{
    public class TcpServer : IServer
    {
        private bool _isRunning;
        public bool IsRunning { get { return _isRunning; } }
        public Action<string> ReadCompleted { get; set; }
        public Action Started { get; set; }
        public Action Stopped { get; set; }

        TcpListener listener;
        public async Task Start()
        {
            listener = new TcpListener(IPAddress.Any, 12000);
            listener.Start();
            _isRunning = true;

            Started?.Invoke();

            while (_isRunning) // Add your exit flag here
            {
                var state = new StateObject();
                try
                {
                    state.client = await listener.AcceptTcpClientAsync();
                    ThreadPool.QueueUserWorkItem(ThreadProc, state);
                }
                catch { }
            }

            Stopped?.Invoke();
        }

        public void Stop()
        {
            listener.Stop();
            _isRunning = false;
        }

        private async void ThreadProc(object obj)
        {
            while (true)
            {
                try
                {
                    var state = (StateObject)obj;

                    int totalBytesRead = 0, bytesRead;
                    int dataLength = 0;
                    var client = state.client as TcpClient;
                    var stream = client.GetStream();
                    bytesRead = await stream.ReadAsync(state.buffer, 0, state.buffer.Length);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    totalBytesRead += bytesRead;
                    dataLength = BitConverter.ToInt32(state.buffer, 0);
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 4, bytesRead - 4));

                    while (totalBytesRead < dataLength)
                    {
                        bytesRead = await stream.ReadAsync(state.buffer, 0, state.buffer.Length);
                        totalBytesRead += bytesRead;
                        state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    }

                    var text = Thread.CurrentThread.ManagedThreadId.ToString() + " " + state.sb.ToString();
                    state.sb.Clear();

                    ReadCompleted?.Invoke(text);

                    if (text == "exit")
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    break;
                }
            }
        }
    }
}
