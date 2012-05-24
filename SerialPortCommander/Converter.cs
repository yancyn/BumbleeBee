using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using GBS.IO;

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
        #region IValueConverter Members
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
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int converted = 0;
            if (value == null) return converted;

            if (value is bool)
                converted = ((bool)value) ? 1 : 0;
            else if (value is int)
                converted = (System.Convert.ToInt32(value) == 1) ? 1 : 0;
            else if (value is string)
                converted = (value.ToString() == "1") ? 1 : 0;

            return converted;
        }
        #endregion
    }
    public class HexConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members
        //integer to hex string
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length > 1)
            {
                int target = (int)values[0];
                SerialCommand command = (SerialCommand)values[1];

                string output = string.Empty;
                if (command == null)
                    output = String.Format("0x{0:x4}", target);
                else
                {
                    if (System.Convert.ToInt32(command.MaxValue) == -1)
                        output = String.Format("0x{0:x8}", target);
                    else
                        output = String.Format("0x{0:x4}", target);
                }

                output = output.Replace("0x", "");
                return output.ToUpper();
            }

            throw new NotImplementedException();
        }
        //revert back to int and SerialCommand object
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            object[] outputs = new object[2];
            if (value is string)
            {
                Int64 output = 0;
                Int64.TryParse(value.ToString().ToLower(), out output);
                if (value.ToString().ToLower().Equals(String.Format("0x{0:x4}", 0))
                    || value.ToString().ToLower().Equals(String.Format("0x{0:x8}", 0)))
                    outputs[0] = 0;
                else
                    outputs[0] = value;//output;// System.Convert.ToInt64(value.ToString().ToLower(), 16);
            }

            return outputs;
        }
        #endregion
    }
    /// <summary>
    /// False to set background as gold otherwise default background color.
    /// </summary>
    public class BackgroundConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool success = (bool)values[0];
            GBS.IO.SerialCommand command = (GBS.IO.SerialCommand)values[1];
            //if (command.Enquiring)
            //{
                if (!command.Success)
                {
                    command.SetError("Fail to read or write");
                    return System.Windows.Media.Brushes.Gold;
                }
            //}

            return null;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
    public class GetNameConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GBS.IO.KeyValuePair<int, string>)
            {
                GBS.IO.KeyValuePair<int, string> pair = (GBS.IO.KeyValuePair<int, string>)value;
                return pair.Value;
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
    public class TrueToEnable : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool) return (bool)value;

            throw new NotImplementedException();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
    public class TrueToDisable : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool) return !(bool)value;

            throw new NotImplementedException();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
    /// <summary>
    /// Converter of ParameterOptions.
    /// </summary>
    public class OptionConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            List<GBS.IO.KeyValuePair<int, string>> options = (List<GBS.IO.KeyValuePair<int, string>>)values[0];
            if (values[1] is GBS.IO.KeyValuePair<int, string>)
            {
                GBS.IO.KeyValuePair<int, string> pair = (GBS.IO.KeyValuePair<int, string>)values[1];
                return pair;
            }
            else
            {
                int value = 0;
                if (values[1] is int)
                    value = (int)values[1];

                foreach (GBS.IO.KeyValuePair<int, string> option in options)
                {
                    if (option.Key == value)
                        return option;
                }
            }

            //todo: OptionConverter write error into log
            return null;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            GBS.IO.KeyValuePair<int, string> input = (GBS.IO.KeyValuePair<int, string>)value;
            object[] values = new object[2];
            values[0] = new List<GBS.IO.KeyValuePair<int, string>> { input };
            values[1] = input.Key;

            return values;
        }
        #endregion
    }
}