using Lana.Domain;
using Lana.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lana.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
    public partial class MainWindow : Window
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
    {
        private NotifyIcon _notifyIcon = null;
        private CancellationTokenSource _cts = null;
        private SpeechListener _speechListener = null;

        public ObservableCollection<Prediction> PredictionsList { get; } = new ObservableCollection<Prediction>();

        public MainWindow()
        {
            InitializeComponent();
            InitializeSpeechListener();
            DataContext = PredictionsList;
        }

        protected override void OnInitialized(EventArgs e)
        {
            Closing += OnClosing;
            Loaded += OnLoaded;

            InitializeNotifyIcon();
            base.OnInitialized(e);
        }

        private void InitializeSpeechListener()
        {
            _cts = new CancellationTokenSource();
            _speechListener = new SpeechListener(PredictionsList, SynchronizationContext.Current);
            var speechListenerTask = Task.Run(() => _speechListener.Run(_cts));
        }

        private void InitializeNotifyIcon()
        {
            _notifyIcon = new NotifyIcon()
            {
                Icon = new Icon("lana_icon.ico"),
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip(),
                Text = "Lana"
            };
            _notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            _notifyIcon.ContextMenuStrip.Items.Add("Exit", null, Exit_Click);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            MinimizeWindow();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            MinimizeWindow();
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.RestoreWindow();
        }

        private bool _autoScroll = true;
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange == 0)
            {
                if ((e.Source as ScrollViewer).VerticalOffset == (e.Source as ScrollViewer).ScrollableHeight)
                    _autoScroll = true;
                else
                    _autoScroll = false;
            }

            if (_autoScroll && e.ExtentHeightChange != 0)
                (e.Source as ScrollViewer).ScrollToVerticalOffset((e.Source as ScrollViewer).ExtentHeight);
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
                _notifyIcon = null;
            }
            System.Windows.Application.Current.Dispatcher.InvokeShutdown();
            return;
        }

        private void MinimizeWindow()
        {
            this.WindowState = WindowState.Minimized;

            this.ShowInTaskbar = false;
            this.Topmost = false;
        }

        private void RestoreWindow()
        {
            if (this.WindowState != WindowState.Normal)
                this.WindowState = WindowState.Normal;

            this.ShowInTaskbar = true;
            this.Topmost = true;
        }
    }
}
