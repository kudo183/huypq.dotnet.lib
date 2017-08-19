using huypq.wpf.Utils;
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
        ScrollViewer _scrollViewer;
        DataManager _dataManager = new DataManager();
        MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        protected override void OnInitialized(EventArgs e)
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()) == true)
            {
                return;
            }

            _viewModel = new MainWindowViewModel(_dataManager);
            
            _viewModel.LoadCommand = new SimpleCommand("LoadCommand", () =>
            {
                gridView.dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
                _viewModel.Load();
            });

            DataContext = _viewModel;

            gridView.MapHeaderFilterModelToColumnHeader(_viewModel);

            base.OnInitialized(e);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var border = (Border)VisualTreeHelper.GetChild(gridView.dataGrid, 0);
            _scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);

            _dataManager.Server.Started = () =>
            {
                Dispatcher.Invoke(() =>
                {
                    btnStart.IsEnabled = false;
                    btnStop.IsEnabled = true;
                });
            };
            _dataManager.Server.Stopped = () =>
            {
                Dispatcher.Invoke(() =>
                {
                    btnStart.IsEnabled = true;
                    btnStop.IsEnabled = false;
                });
            };
        }

        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            await _dataManager.Server.Start();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            _dataManager.Server.Stop();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            _dataManager.ClearData();
            _viewModel.Load();
        }

        OpenFileDialog ofd = new OpenFileDialog();

        private async void Load_Click(object sender, RoutedEventArgs e)
        {
            _dataManager.Server.Stop();

            if (ofd.ShowDialog() == true)
            {
                await _dataManager.LoadFromFile(ofd.FileName);
                _viewModel.Load();
            }
        }
    }
}
