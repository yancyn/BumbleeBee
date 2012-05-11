using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SerialPortCommander
{
    /// <summary>
    /// If there is zero row count just hidden otherwise visible.
    /// </summary>
    public class VisibilityConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                return ((int)value) > 0 ? Visibility.Visible : Visibility.Hidden;
            }
            else if (value == null)
            {
                return Visibility.Visible;
            }

            throw new ArgumentException("Not supported type of " + value.GetType());
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
    /// <summary>
    /// If 1 then return true otherwise false.
    /// </summary>
    public class BooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool actual = false, passin = false;
            if (value == null) return actual;

            if (value is bool)
                actual = (bool)value;
            else if (value is int)
                actual = ((int)value == 1) ? true : false;
            else if (value is string)
                actual = (value.ToString() == "1") ? true : false;

            if (parameter is bool)
                passin = (bool)parameter;
            else if (parameter is int)
                passin = ((int)parameter == 1) ? true : false;
            else if (parameter is string)
                passin = (parameter.ToString() == "1") ? true : false;

            return (actual == passin);
            throw new NotImplementedException();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //int converted = 0;
            //if (value == null) return converted;

            //if (value is bool)
            //    converted = ((bool)value) ? 1 : 0;
            //else if (value is int)
            //    converted = (int)value;
            //else if (value is string)
            //    converted = (value.ToString() == "1") ? 1 : 0;
            //return converted;

            //throw new NotImplementedException();

            return null;
        }
    }
    /// <summary>
    /// False to set background as red otherwise default background color.
    /// </summary>
    public class BackgroundConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value is Boolean)
            //{
            //    return ((bool)value)
            //        ? System.Windows.Media.Brushes.White : System.Windows.Media.Brushes.Red;
            //}

            bool success = (bool)values[0];
            GBS.IO.SerialCommand command = (GBS.IO.SerialCommand)values[1];
            if (command.Enquiring)
            {
                if (!command.Success)
                {
                    command.SetError("Fail to read or write");
                    return System.Windows.Media.Brushes.Red;
                }
            }

            return null;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}