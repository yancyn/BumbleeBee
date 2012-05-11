using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace GBS.IO
{
    public partial class SerialCommand : IDataErrorInfo
    {
        #region Properties
        private bool hasChanged;
        /// <summary>
        /// Determine whether this parameter command has been modified since load from setting.
        /// </summary>
        public bool HasChanged { get { return this.hasChanged; } }
        private string errorField;
        #endregion

        #region Constructors
        public SerialCommand()
        {
            Initialize();
        }
        public SerialCommand(string name, ParameterType type)
        {
            Initialize();

            this.nameField = name;
            this.parameterTypeField = type;
            InitialDefault();
        }
        public SerialCommand(string name, Object value, ParameterType type)
        {
            Initialize();

            this.nameField = name;
            this.parameterTypeField = type;
            InitialDefault();

            this.parameterValueField = value;
        }
        public SerialCommand(string name, Object value)
        {
            Initialize();

            this.nameField = name;
            this.parameterValueField = value;
        }
        public SerialCommand(string name, Object value, Object maxValue)
        {
            Initialize();

            this.nameField = name;
            this.parameterValueField = value;
            this.maxValueField = maxValue;
            InitialMin();
        }
        public SerialCommand(string name, Object value, string unit)
        {
            Initialize();

            this.nameField = name;
            this.parameterValueField = value;
            this.unitField = unit;
        }
        public SerialCommand(string name, Object value, Object maxValue, string unit)
        {
            Initialize();

            this.nameField = name;
            this.parameterValueField = value;
            this.unitField = unit;
            this.maxValueField = maxValue;
            InitialMin();
        }
        #endregion

        #region Methods
        private void Initialize()
        {
            this.parameterTypeField = ParameterType.Integer;
            this.maxValueField = new Object();
            this.minValueField = new Object();
            this.parameterOptionsField = new List<KeyValuePair<int, string>>();
            this.parameterValueField = new Object();
            this.hasChanged = false;
            this.enquiringField = false;
            this.successField = false;
        }
        private void InitialDefault()
        {
            switch (parameterTypeField)
            {
                case ParameterType.String:
                    this.minValueField = string.Empty;
                    this.maxValueField = string.Empty;
                    break;
                case IO.ParameterType.Integer:
                    this.minValueField = 0;
                    this.maxValueField = 0;
                    break;
                case IO.ParameterType.Hex:
                    this.minValueField = 0x0000;
                    this.maxValueField = 0xFFFF;
                    break;
            }
        }
        private void InitialMin()
        {
            if (maxValueField.GetType() == typeof(Int32))
                this.minValueField = 0;
            else if (maxValueField.GetType() == typeof(int))
                this.minValueField = 0;
            else if (maxValueField.GetType() == typeof(decimal))
                this.minValueField = 0.0d;
            else if (maxValueField.GetType() == typeof(string))
                this.minValueField = string.Empty;
            //todo: initial minValue for hex type
        }
        /// <summary>
        /// Return to original state when first load the value.
        /// </summary>
        public void ResetState()
        {
            this.hasChanged = false;
            OnPropertyChanged("HasChanged");
        }
        public void SetEnquiring(bool inquired)
        {
            this.enquiringField = inquired;
            OnPropertyChanged("Enquiring");
        }
        public void SetSuccess()
        {
            System.Diagnostics.Debug.WriteLine("Set " + this.nameField + " to success");
            this.successField = true;
            this.enquiringField = false;//reset
            OnPropertyChanged("Success");
        }
        public void SetFail()
        {
            System.Diagnostics.Debug.WriteLine("Set " + this.nameField + " to fail");
            this.successField = false;
            OnPropertyChanged("Success");
        }
        public void SetError(string message)
        {
            this.errorField = message;
            OnPropertyChanged("Success");
        }
        #endregion

        #region IDataErrorInfo members
        public string Error
        {
            get { return string.Empty; }
        }
        //TODO: IDataErrorInfo
        public string this[string columnName]
        {
            get
            {
                this.errorField = string.Empty;
                if (this.hasChanged)
                {
                    switch (columnName)
                    {
                        case "ParameterValue":
                            switch (this.parameterTypeField)
                            {
                                case ParameterType.String:
                                    break;
                                case ParameterType.Integer:
                                    Int32 min = (Int32)this.minValueField;
                                    Int32 max = (Int32)this.maxValueField;
                                    Int32 value = 0;
                                    Int32.TryParse(parameterValueField.ToString(), out value);
                                    if (value == 0 || value < min || value > max)
                                        this.errorField = string.Format("Value must between {0} and {1}.", min, max);
                                    break;
                                case ParameterType.Hex:
                                    break;
                            }
                            break;
                    }
                }

                return this.errorField;
            }
        }
        #endregion
    }
}