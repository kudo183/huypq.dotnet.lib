using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace huypq.Logging
{
    public class ElasticsearchBatchWriter : ILogBatchWriter
    {
        readonly string _url;
        readonly string _index;
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

        public ElasticsearchBatchWriter(string url, string index, string user, string pass)
        {
            _url = url;
            _index = index;
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", user, pass))));
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
            var url = string.Format("{0}/{1}-{2:yyyy.MM.dd}/_doc/_bulk", _url, _index, System.DateTime.UtcNow);
            var result = await HttpClient.PostAsync(url, content);
        }

        public async Task<bool> WriteTest()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Index);
            sb.Append("{\"msg\":\"test\"}");
            sb.Append("\n");
            var content = new StringContent(sb.ToString(), Encoding.UTF8, "application/json");
            var now = DateTime.UtcNow;
            var url = string.Format("{0}/{1}-{2:yyyy.MM.dd}/_doc/_bulk", _url, _index, now);
            var result = await HttpClient.PostAsync(url, content);
            var resultString = await result.Content.ReadAsStringAsync();
            Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(resultString);

            if (obj.Value<bool>("errors") == false)
            {
                await HttpClient.DeleteAsync(string.Format("{0}/{1}-{2:yyyy.MM.dd}", _url, _index, now));
                return true;
            }
            return false; ;
        }
    }
}
