using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using GBS.IO;

namespace SerialPortCommander
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //#region Properties
        //protected SplashScreen splash;
        //Thread startupThread;
        //public QualityQuestDataViewModel qqDataViewModel { get; set; }
        //AutoResetEvent splashCreatedEvent = new AutoResetEvent(false);
        //#endregion

        //private delegate void VoidHandler();

        //private void StartSplashThread()
        //{
        //    Dispatcher.CurrentDispatcher.BeginInvoke((Action)delegate()
        //    {
        //        AssemblyInfo assemblyInfo = new AssemblyInfo();
        //        splash = new SplashScreen();
        //        splash.DataContext = assemblyInfo;//to display application assembly info
        //        splash.Show();
        //        splashCreatedEvent.Set();
        //    });

        //    Dispatcher.Run();
        //}

        //public void ShowSplashScreen()
        //{
        //    Thread uiThread = new Thread(StartSplashThread);
        //    uiThread.SetApartmentState(ApartmentState.STA);
        //    uiThread.IsBackground = true;
        //    uiThread.Start();
        //    splashCreatedEvent.WaitOne();
        //    qqDataViewModel = new QualityQuestDataViewModel(splash);
        //    startupThread = new Thread(new ThreadStart(qqDataViewModel.Initialize));
        //    startupThread.Priority = ThreadPriority.Highest;
        //    startupThread.Start();
        //}

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //qqDataViewModel.loadedEvent.WaitOne();
            //new QualityQuestWindow(qqDataViewModel).Show();
        }
        private void App_Startup(object sender, StartupEventArgs e)
        {
            foreach (string arg in e.Args)
                Logger.Debug(typeof(App), "argument");
        }
    }

    partial class EntryPoint
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        private static Mutex m_Mutex;

        [STAThread]
        public static void Main()
        {
            bool createdNew;
            //MainWindow app = null;
            //Logger.Debug(typeof(App), "Launching SLIQ Application (Entry Point)");
            try
            {
                m_Mutex = new Mutex(true, "SerialPortCommanderStartupMutex", out createdNew);
                if (createdNew)
                {
                    SerialPortCommander.App splashApp = new SerialPortCommander.App();
                    splashApp.InitializeComponent();
                    //TODO: splashApp.ShowSplashScreen();
                    splashApp.Run();
                }
                else
                {
                    // Bring other instance into view. 
                    System.Diagnostics.Process current = System.Diagnostics.Process.GetCurrentProcess();
                    foreach (System.Diagnostics.Process process in System.Diagnostics.Process.GetProcessesByName(current.ProcessName))
                    {
                        if (process.Id != current.Id)
                        {
                            SetForegroundWindow(process.MainWindowHandle);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(typeof(EntryPoint), ex);
                //new ErrorWindow().ShowDialog(ex);

                // Shutting down the application as an unrecoverable error has occurred.
                //if (app != null) app.Close();
            }
        }
    }
}