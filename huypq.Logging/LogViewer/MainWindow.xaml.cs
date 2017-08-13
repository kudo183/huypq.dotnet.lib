using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LogViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ScrollViewer scrollViewer;
        ObservableCollection<LogMessage> listBoxItemSource = new ObservableCollection<LogMessage>();
        //IServer server = new TcpServer();
        IServer server = new UdpServer();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = listBoxItemSource;
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var border = (Border)VisualTreeHelper.GetChild(listBox, 0);
            scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
        }

        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            server.Started = () =>
            {
                Dispatcher.Invoke(() =>
                {
                    btnStart.IsEnabled = false;
                    btnStop.IsEnabled = true;
                });
            };
            server.Stopped = () =>
            {
                Dispatcher.Invoke(() =>
                {
                    btnStart.IsEnabled = true;
                    btnStop.IsEnabled = false;
                });
            };
            server.ReadCompleted = (text) =>
            {
                Dispatcher.Invoke(() =>
                {
                    AddItem(text);
                });
            };
            await server.Start();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            server.Stop();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            listBoxItemSource.Clear();
        }

        private void AddItem(string text)
        {
            var log = JsonConvert.DeserializeObject<LogMessage>(text);

            listBoxItemSource.Add(log);
            if (listBoxItemSource.Count > 50)
            {
                listBoxItemSource.RemoveAt(0);
            }
            scrollViewer.ScrollToBottom();
        }

        OpenFileDialog ofd = new OpenFileDialog();

        private async void Load_Click(object sender, RoutedEventArgs e)
        {
            server.Stop();

            if (ofd.ShowDialog() == true)
            {
                using (var sr = System.IO.File.OpenText(ofd.FileName))
                {
                    var sb = new StringBuilder();
                    while (sr.EndOfStream == false)
                    {
                        var text = await sr.ReadLineAsync();
                        sb.Append(text);
                        while (text.EndsWith("}") == false)
                        {
                            text = await sr.ReadLineAsync();
                            sb.Append(text);
                        }

                        listBoxItemSource.Add(JsonConvert.DeserializeObject<LogMessage>(sb.ToString()));
                        sb.Clear();
                    }
                    scrollViewer.ScrollToBottom();
                }
            }
        }
    }
}
