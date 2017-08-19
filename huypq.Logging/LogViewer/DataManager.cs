using huypq.QueryBuilder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogViewer
{
    public class DataManager
    {
        const int maxBufferSize = 1000;
        ObservableCollection<LogMessage> dataBuffer = new ObservableCollection<LogMessage>();
        //IServer server = new TcpServer();
        IServer server = new UdpServer();

        public IServer Server { get { return server; } }

        public DataManager()
        {
            server.ReadCompleted = (text) =>
            {
                var log = JsonConvert.DeserializeObject<LogMessage>(text);

                dataBuffer.Add(log);

                if (dataBuffer.Count > maxBufferSize)
                {
                    dataBuffer.RemoveAt(0);
                }
            };
        }

        public async Task LoadFromFile(string fileName)
        {
            using (var sr = System.IO.File.OpenText(fileName))
            {
                var sb = new StringBuilder();
                dataBuffer.Clear();
                while (sr.EndOfStream == false)
                {
                    var text = await sr.ReadLineAsync();
                    sb.Append(text);
                    while (text.EndsWith("}") == false)
                    {
                        text = await sr.ReadLineAsync();
                        sb.Append(text);
                    }

                    dataBuffer.Add(JsonConvert.DeserializeObject<LogMessage>(sb.ToString()));
                    sb.Clear();
                }
            }
        }

        public void ClearData()
        {
            dataBuffer.Clear();
        }

        public List<LogMessage> GetData(ref QueryExpression qe, out int pageCount)
        {
            return QueryExpression.AddQueryExpression(dataBuffer.AsQueryable(), ref qe, out pageCount).ToList();
        }
    }
}
