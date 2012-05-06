﻿using System;
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
        }
        /// <summary>
        /// Return to original state when first load the value.
        /// </summary>
        public void ResetState()
        {
            this.hasChanged = false;
        }
        public void SetInquired(bool inquired)
        {
            this.isInquiredField = inquired;
        }
        public void SetSuccess()
        {
            this.successField = true;
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
                string error = string.Empty;
                switch (columnName)
                {
                    case "ParameterValue":
                        switch (this.parameterTypeField)
                        {
                            case ParameterType.String:
                                break;
                            case ParameterType.Integer:
                                if (minValueField != null && maxValueField != null && parameterValueField != null)
                                {
                                    Int32 min = (Int32)this.minValueField;
                                    Int32 max = (Int32)this.maxValueField;
                                    Int32 value = 0;
                                    Int32.TryParse(parameterValueField.ToString(), out value);
                                    if (value == 0)
                                        error = "Value must be an integer.";
                                    else if (value < min || value > max)
                                    {
                                        error = string.Format("Value must between {0} and {1}.", min, max);
                                    }
                                }
                                break;
                            case ParameterType.Hex:
                                break;
                        }
                        break;
                }

                return error;
            }
        }
        #endregion
    }
}