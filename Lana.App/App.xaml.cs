using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Lana.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex _mutex = new Mutex(true, "Lana");
        private static MainWindow _mainWindow = null;

        public App()
        {
            InitializeComponent();
        }

        [STAThread]
        public static void Main()
        {
            if (_mutex.WaitOne(TimeSpan.Zero, true))
            {
                var app = new App();
                _mainWindow = new MainWindow();
                app.Run(_mainWindow);
                _mutex.ReleaseMutex();
            }
        }
    }
}
