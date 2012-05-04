using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBS.IO
{
    public partial class SerialCommand
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
        }
        public SerialCommand(string name, Object value, ParameterType type)
        {
            Initialize();

            this.nameField = name;
            this.parameterValueField = value;
            this.parameterTypeField = type;
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
    }
}