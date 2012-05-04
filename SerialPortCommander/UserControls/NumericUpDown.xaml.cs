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

namespace SerialPortCommander
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        /// <summary>
        /// Gets or sets value to display.
        /// </summary>
        public Int32 Value
        {
            get { return (Int32)GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);
                OnPropertyChanged("Value");
            }
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(object),
            typeof(NumericUpDown));

        /// <summary>
        /// Gets or sets mininum value for this control.
        /// </summary>
        public int MinValue
        {
            get { return (Int32)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            "MinValue",
            typeof(Int32),
            typeof(NumericUpDown));


        /// <summary>
        /// Gets or sets maximum value for this control.
        /// </summary>
        public int MaxValue
        {
            get { return (Int32)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue",
            typeof(Int32),
            typeof(NumericUpDown));

        public NumericUpDown()
        {
            InitializeComponent();
            //txtNum.Text = this.Value.ToString();
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler handler = this.PropertyChanged;
            if ((handler != null))
            {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
                switch (propertyName)
                {
                    case "Value":
                        //ValueChanged();
                        break;
                }
            }
        }
        private void cmdUp_Click(object sender, RoutedEventArgs e)
        {
            Value++;
            this.Value = Math.Min(this.Value, this.MaxValue);
            System.Diagnostics.Debug.WriteLine(this.Value);
        }
        private void cmdDown_Click(object sender, RoutedEventArgs e)
        {
            Value--;
            this.Value = Math.Max(this.Value, this.MinValue);
        }
        private void ValueChanged()
        {
            if (this.Value > this.MaxValue)
                this.Value = this.MaxValue;
            else if (this.Value < this.MinValue)
                this.Value = this.MinValue;
        }
        private void txtNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            int value = 0;
            if (int.TryParse(txtNum.Text, out value))
                this.Value = value;
            //ValueChanged();
        }
    }
}