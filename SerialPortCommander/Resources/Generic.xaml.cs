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
using System.Windows.Shapes;
using GBS.IO;

namespace SerialPortCommander
{
    /// <summary>
    /// Interaction logic for Generic.xaml
    /// </summary>
    public partial class Generic : ResourceDictionary
    {
        public Generic()
        {
            InitializeComponent();
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine((sender as RadioButton).DataContext);
            //SerialCommand command = (SerialCommand)(sender as RadioButton).DataContext;
            //command.ParameterValue = Convert.ToInt32((sender as RadioButton).GroupName);
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine((sender as RadioButton).DataContext);
        }
    }
}