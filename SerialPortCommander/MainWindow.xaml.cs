using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using GBS.IO;

namespace SerialPortCommander
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialCommander commander;

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        #region Methods
        /// <summary>
        /// Initial Windows layout.
        /// </summary>
        private void Initialize()
        {
            this.Title = Properties.Settings.Default.Company + " Studio";
            commander = new SerialCommander("BumbleBee");
            //commander.ImportSetting("current.serial");
            commander.LoadSetting();
            MainGrid.DataContext = commander;
        }
        #endregion

        #region Events
        private void menuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem[] menus = new MenuItem[3] { menuModem, menuPager, menuPTX };
            foreach (MenuItem menu in menus)
            {
                if (menu.Header.Equals((sender as MenuItem).Header))
                    menu.IsChecked = true;
                else
                    menu.IsChecked = false;
            }
        }
        private void MenuSetting_Click(object sender, RoutedEventArgs e)
        {
            //TODO: click on menu setting
        }
        private void MenuPrint_Click(object sender, RoutedEventArgs e)
        {
            //TODO: print out
            System.Diagnostics.Debug.WriteLine("printing...");
        }
        private void MenuClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            string version = "Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            MessageBox.Show("GBS Embeded Solution" + "\n" + version);
        }
        private void ReportBugLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Hyperlink hyperlink = (sender as Hyperlink);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(hyperlink.NavigateUri.ToString()));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            commander.Dispose();
        }
        #endregion
    }
}