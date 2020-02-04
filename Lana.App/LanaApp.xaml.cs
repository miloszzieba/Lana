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
    public partial class LanaApp : Application
    {
#if DEBUG
        private static Mutex _mutex = new Mutex(true, Guid.NewGuid().ToString());
#endif
#if RELEASE
        private static Mutex _mutex = new Mutex(true, "Lana");
#endif
        private static MainWindow _mainWindow = null;

        public LanaApp()
        {
            InitializeComponent();
        }

        [STAThread]
        public static void Main()
        {
            if (_mutex.WaitOne(TimeSpan.Zero, true))
            {
                var app = new LanaApp();
                _mainWindow = new MainWindow();
                app.Run(_mainWindow);
                _mutex.ReleaseMutex();
            }
        }
    }
}
