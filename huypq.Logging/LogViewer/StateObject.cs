using System.Text;

namespace LogViewer
{
    // State object for reading client data asynchronously  
    public class StateObject
    {
        // Client  socket.  
        public object client = null;
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }

}
