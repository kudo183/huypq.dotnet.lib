using huypq.wpf.Utils;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LogViewer
{
    /// <summary>
    /// Interaction logic for LogViewerControl.xaml
    /// </summary>
    public partial class LogViewerControl : UserControl
    {
        ScrollViewer _scrollViewer;
        DataManager _dataManager = new DataManager();
        LogViewerControlViewModel _viewModel;
        bool _isInDesignMode;

        public LogViewerControl()
        {
            InitializeComponent();
            _isInDesignMode = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());

            if (_isInDesignMode == true)
            {
                return;
            }

            Loaded += MainWindow_Loaded;
            Unloaded += LogViewerControl_Unloaded;
        }

        private void LogViewerControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_dataManager != null && _dataManager.Server.IsRunning == true)
            {
                _dataManager.Server.Stop();
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            if (_isInDesignMode == true)
            {
                return;
            }

            _viewModel = new LogViewerControlViewModel(_dataManager);

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
