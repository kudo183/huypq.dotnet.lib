using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace huypq.Logging
{
    public class ElasticsearchBatchWriter : ILogBatchWriter
    {
        readonly string _url;
        HttpClient _httpClient;
        const string Index = "{\"index\":{}}\n";

        public ElasticsearchBatchWriter(string url, string index)
        {
            _url = url + "/" + index + "/_doc/_bulk";
            _httpClient = new HttpClient();
        }

        public async Task Write(List<LogEntry> logEntries)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var log in logEntries)
            {
                sb.Append(Index);
                sb.Append(log.Message);
            }
            var content = new StringContent(sb.ToString(), Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync(_url, content);
        }
    }
}
