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
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //save the last selected device mode into config file
            int index = 0;
            string[] fileNames = SerialCommander.GetDefaultCurrentFileNames();
            MenuItem[] menus = new MenuItem[3] { menuModem, menuPager, menuPTX };
            for (int i = 0; i < menus.Length; i++)
            {
                if (menus[i].IsChecked)
                    index = i;
            }
            Properties.Settings.Default.Entry = fileNames[index];
            Properties.Settings.Default.Save();

            commander.Dispose();
        }

        #region Methods
        /// <summary>
        /// Initial Windows layout.
        /// </summary>
        private void Initialize()
        {
            this.Title = Properties.Settings.Default.Company + " Studio" + " ver " + GetAssemblyVersion("SerialPortCommander.exe", 3);
            commander = new SerialCommander("BumbleBee");
            commander.LoadSetting(Properties.Settings.Default.Entry);
            MainGrid.DataContext = commander;

            //check the selected mode based on file name loaded
            MenuItem[] menus = new MenuItem[3] { menuModem, menuPager, menuPTX };
            int index = 0;
            string fileName = Properties.Settings.Default.Entry.TrimEnd(
                System.IO.Path.GetExtension(SerialCommander.DEFAULT_FILENAME).ToCharArray());
            fileName = fileName.TrimStart(
                System.IO.Path.GetFileNameWithoutExtension(SerialCommander.DEFAULT_FILENAME).ToCharArray());
            if (fileName.Length > 0) index = Convert.ToInt32(fileName);
            menus[index].IsChecked = true;
        }
        public string GetAssemblyVersion(string assemblyName, int digit)
        {
            string lVersion = "";
            assemblyName = assemblyName.Trim().ToUpper();
            if (digit > 4) digit = 4;//maximum

            try
            {
                System.Reflection.Assembly[] lAssemblies = System.Threading.Thread.GetDomain().GetAssemblies();
                foreach (System.Reflection.Assembly ass in lAssemblies)
                {
                    if (ass.GetType() == typeof(System.Reflection.Assembly)
                        && ass.CodeBase.ToUpper().LastIndexOf(assemblyName) > -1)
                    {
                        string[] lSplits = ass.FullName.Split(' ');
                        foreach (string s in lSplits)
                        {
                            if (s.Trim(',').ToLower().IndexOf("version") > -1)
                            {
                                string[] lVers = s.Trim(',').Split('=');
                                lVersion = lVers[lVers.Length - 1];
                                if (digit < 4) //only less than 4 digit
                                {
                                    lVers = lVersion.Split('.');
                                    lVersion = "";//reset
                                    for (int i = 0; i < digit; i++)
                                        lVersion += lVers[i] + ".";
                                }
                            }
                        }
                    }//found specified dll only
                }//end loops

                return lVersion.TrimEnd('.');
            }
            catch (Exception ex)
            {
                Logger.Error(typeof(MainWindow), ex);
                return string.Empty;
            }
        }
        #endregion

        #region Events
        private void LoadFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".serial";
            dialog.Filter = "Serial setting (.serial)|*.serial";
            if (dialog.ShowDialog() == true)
                commander.ImportSetting(dialog.FileName);
        }
        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.DefaultExt = ".serial";
            dialog.Filter = "Serial setting (.serial)|*.serial";
            if (dialog.ShowDialog() == true)
                commander.ExportSetting(dialog.FileName);
        }
        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            //open up file dialog
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".bin";
            dialog.Filter = "Binary (.bin)|*.bin";
            if (dialog.ShowDialog() == true)
                pathText.Text = dialog.FileName;
        }

        private void ComPort_Click(object sender, RoutedEventArgs e)
        {
            SerialPortWindow dialog = new SerialPortWindow();
            dialog.Owner = this;
            dialog.DataContext = commander;
            if (dialog.ShowDialog().Value == true)
            {
            }
        }

        private void ModeMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem[] menus = new MenuItem[3] { menuModem, menuPager, menuPTX };
            foreach (MenuItem menu in menus)
            {
                if (menu.Header.Equals((sender as MenuItem).Header))
                    menu.IsChecked = true;
                else
                    menu.IsChecked = false;
            }

            tabItem3.Visibility = (menuPTX.IsChecked)
                ? System.Windows.Visibility.Visible : tabItem3.Visibility = System.Windows.Visibility.Collapsed;
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
            //string version = "Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string version = "version " + GetAssemblyVersion("SerialPortCommander.exe", 3);
            MessageBox.Show("GBS Embeded Solution" + "\n" + version);
        }
        private void ReportBugLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Hyperlink hyperlink = (sender as Hyperlink);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(hyperlink.NavigateUri.ToString()));
        }
        #endregion
    }
}