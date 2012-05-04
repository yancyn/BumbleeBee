using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using GBS.IO;

namespace SerialPortCommander
{
    /// <summary>
    /// Determine which data template to use by SerialCommand attributes.
    /// </summary>
    public class LayoutSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            if (element != null && item != null)
            {
                SerialCommand command = item as SerialCommand;
                if (command.ParameterOptions.Count == 2)
                    return element.FindResource("RadioTemplate") as DataTemplate;
                else if (command.ParameterOptions.Count > 2)
                    return element.FindResource("OptionsTemplate") as DataTemplate;

                switch (command.ParameterType)
                {
                    case ParameterType.String:
                        return element.FindResource("TextTemplate") as DataTemplate;
                    case ParameterType.Integer:
                        return element.FindResource("IntegerTemplate") as DataTemplate;
                    case ParameterType.Hex:
                        break;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }

    public class RadioSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            if (element != null && item != null)
            {
                if (item is GBS.IO.KeyValuePair<Int32, String>)
                {
                    GBS.IO.KeyValuePair<Int32, String> pair = (GBS.IO.KeyValuePair<Int32, String>)item;
                    if (pair.Key == 1)
                        return element.FindResource("PositiveRadioTemplate") as DataTemplate;
                    else
                        return element.FindResource("NegativeRadioTemplate") as DataTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}