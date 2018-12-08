using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace huypq.Logging
{
    public class ElasticsearchBatchWriter : ILogBatchWriter
    {
        static System.DateTime LastDeleteDate = new System.DateTime();

        readonly string _url;
        readonly string _index;
        readonly int _daysOfLog = 7;
        private static readonly HttpClient HttpClient;
        const string Index = "{\"index\":{}}\n";

        static ElasticsearchBatchWriter()
        {
            HttpClient = new HttpClient();
        }

        public ElasticsearchBatchWriter(string url, string index)
        {
            _url = url;
            _index = index;
        }

        public ElasticsearchBatchWriter(string url, string index, int daysOfLog) : this(url, index)
        {
            _daysOfLog = daysOfLog;
        }

        public async Task Write(List<LogEntry> logEntries)
        {
            var now = System.DateTime.UtcNow.AddDays(-_daysOfLog);
            var currentLastDate = new System.DateTime(now.Year, now.Month, now.Day);
            if (LastDeleteDate < currentLastDate)
            {
                await HttpClient.DeleteAsync(string.Format("{0}/{1}-{2:yyyy.MM.dd}", _url, _index, currentLastDate));
                LastDeleteDate = currentLastDate;
            }
            StringBuilder sb = new StringBuilder();
            foreach (var log in logEntries)
            {
                sb.Append(Index);
                sb.Append(log.Message);
            }
            var content = new StringContent(sb.ToString(), Encoding.UTF8, "application/json");
            var url = string.Format("{0}/{1}-{2:yyyy.MM.dd}/_doc/_bulk", _url, _index, System.DateTime.UtcNow);
            var result = await HttpClient.PostAsync(url, content);
        }
    }
}
