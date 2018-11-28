using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace huypq.Logging
{
    public class FileBatchWriter : ILogBatchWriter
    {
        readonly string _path;
        readonly string _fileNamePrefix;
        readonly int _maxRetainedFiles;
        readonly int _maxFileSize;

        public FileBatchWriter(string path, string fileNamePrefix, int maxRetainedFiles, int maxFileSize)
        {
            _path = path;
            _fileNamePrefix = fileNamePrefix;
            _maxRetainedFiles = maxRetainedFiles;
            _maxFileSize = maxFileSize;
        }

        public async Task Write(List<LogEntry> logEntries)
        {
            Directory.CreateDirectory(_path);

            foreach (var group in logEntries.GroupBy(GetGrouping))
            {
                var fullName = GetFullName(group.Key);
                var fileInfo = new FileInfo(fullName);
                if (_maxFileSize > 0 && fileInfo.Exists && fileInfo.Length > _maxFileSize)
                {
                    return;
                }

                using (var streamWriter = File.AppendText(fullName))
                {
                    foreach (var item in group)
                    {
                        await streamWriter.WriteAsync(item.Message);
                    }
                }
            }

            RollFiles();
        }

        private string GetFullName(string group)
        {
            return System.IO.Path.Combine(_path, $"{_fileNamePrefix}{group}.txt");
        }

        string GetGrouping(LogEntry message)
        {
            return $"{message.Timestamp.Year:0000}{message.Timestamp.Month:00}{message.Timestamp.Day:00}";
        }

        protected void RollFiles()
        {
            if (_maxRetainedFiles > 0)
            {
                var files = new DirectoryInfo(_path)
                    .GetFiles(_fileNamePrefix + "*")
                    .OrderByDescending(f => f.Name)
                    .Skip(_maxRetainedFiles);

                foreach (var item in files)
                {
                    item.Delete();
                }
            }
        }
    }
}
