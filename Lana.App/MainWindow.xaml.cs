using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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
    public partial class MainWindow : Window
    {
        private NotifyIcon _notifyIcon = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            Loaded += OnLoaded;
            StateChanged += OnStateChanged;
            Closing += OnClosing;
            Closed += OnClosed;

            _notifyIcon = new NotifyIcon()
            {
                Icon = new Icon("lana_icon.ico"),
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip(),
                Text = "Lana"
            };
            _notifyIcon.Click += NotifyIcon_Click;
            _notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            _notifyIcon.ContextMenuStrip.Items.Add("Exit", null, Exit_Click);

            this.Visibility = Visibility.Hidden;
            this.WindowState = WindowState.Minimized;
            this.Topmost = false;
            this.ShowInTaskbar = false;

            base.OnInitialized(e);
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Visible;
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                this.Topmost = false;
            }
            else
            {
                this.ShowInTaskbar = true;
                this.Topmost = true;
            }
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.WindowState != WindowState.Minimized)
            {
                e.Cancel = true;
                this.WindowState = WindowState.Minimized;
            }
        }

        private void OnClosed(object sender, System.EventArgs e)
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
                _notifyIcon = null;
            }
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowWindow();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
            return;
        }

        private void ShowWindow()
        {
            if (this.WindowState == WindowState.Minimized)
                this.WindowState = WindowState.Normal;

            this.Show();
        }
    }
}
