using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO.Ports;
using System.Reflection;
using System.ComponentModel;
using System.Threading;
using System.IO;
using System.Windows.Input;

namespace GBS.IO
{
    /// <summary>
    /// Manager for serial port data
    /// </summary>
    public class SerialPortManager : IDisposable, INotifyPropertyChanged
    {
        #region Fields
        private SerialPort _serialPort;
        private SerialSettings _currentSerialSettings = new SerialSettings();
        private string _latestRecieved = String.Empty;
        public event EventHandler<SerialDataEventArgs> NewSerialDataRecieved;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the current serial port settings
        /// </summary>
        public SerialSettings CurrentSerialSettings
        {
            get { return _currentSerialSettings; }
            set { _currentSerialSettings = value; }
        }
        /// <summary>
        /// Indicate the current port is open for connected.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                if (_serialPort == null) return false;
                return _serialPort.IsOpen;
            }
        }
        #endregion

        public SerialPortManager()
        {
            // Finding installed serial ports on hardware
            string[] ports = ValidatePortNames(SerialPort.GetPortNames());
            _currentSerialSettings.PortNameCollection = ports;
            _currentSerialSettings.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_currentSerialSettings_PropertyChanged);

            // If serial ports is found, we select the first found
            if (_currentSerialSettings.PortNameCollection.Length > 0)
                _currentSerialSettings.PortName = _currentSerialSettings.PortNameCollection[0];
        }
        ~SerialPortManager()
        {
            Dispose(false);
        }

        #region Event handlers
        void _currentSerialSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // if serial port is changed, a new baud query is issued
            if (e.PropertyName.Equals("PortName"))
                UpdateBaudRateCollection();
        }
        void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int dataLength = _serialPort.BytesToRead;
            byte[] data = new byte[dataLength];
            int nbrDataRead = _serialPort.Read(data, 0, dataLength);
            if (nbrDataRead == 0)
                return;

            // Send data to whom ever interested
            if (NewSerialDataRecieved != null)
                NewSerialDataRecieved(this, new SerialDataEventArgs(data));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Connects to a serial port defined through the current settings
        /// </summary>
        public void StartListening()
        {
            // Closing serial port if it is open
            StopListening();

            // Setting serial port settings
            _serialPort = new SerialPort(
                _currentSerialSettings.PortName,
                _currentSerialSettings.BaudRate,
                _currentSerialSettings.Parity,
                _currentSerialSettings.DataBits,
                _currentSerialSettings.StopBits);

            // Subscribe to event and open serial port for data
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(_serialPort_DataReceived);
            _serialPort.Open();
            OnPropertyChanged("IsOpen");
        }
        /// <summary>
        /// Closes the serial port
        /// </summary>
        public void StopListening()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.DataReceived -= new SerialDataReceivedEventHandler(_serialPort_DataReceived);
                _serialPort.Close();
                OnPropertyChanged("IsOpen");
            }
        }
        /// <summary>
        /// Retrieves the current selected device's COMMPROP structure, and extracts the dwSettableBaud property
        /// </summary>
        private void UpdateBaudRateCollection()
        {
            _serialPort = new SerialPort(_currentSerialSettings.PortName);

            try
            {
                _serialPort.Open();
                object p = _serialPort.BaseStream.GetType().GetField("commProp", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_serialPort.BaseStream);
                Int32 dwSettableBaud = (Int32)p.GetType().GetField("dwSettableBaud", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(p);

                _serialPort.Close();
                _currentSerialSettings.UpdateBaudRateCollection(dwSettableBaud);
            }
            catch (Exception ex)
            {
                Logger.Error(typeof(SerialPortManager), ex);
                return;
            }
        }
        /// <summary>
        /// Write data to port.
        /// </summary>
        public void Write(string text)
        {
            _serialPort.Write(text);
        }

        // Call to release serial port
        public void Dispose()
        {
            Dispose(true);
        }
        // Part of basic design pattern for implementing Dispose
        protected virtual void Dispose(bool disposing)
        {
            // Releasing serial port (and other unmanaged objects)
            if (_serialPort != null)
            {
                if (disposing)
                {
                    _serialPort.DataReceived -= new SerialDataReceivedEventHandler(_serialPort_DataReceived);
                }

                if (_serialPort.IsOpen)
                    _serialPort.Close();

                _serialPort.Dispose();
            }
        }
        #endregion

        #region Functions
        /// <summary>
        /// Validate the result passed back from .NET framework. There might be odd setting on local machine.
        /// </summary>
        /// <param name="ports"></param>
        /// <returns></returns>
        private string[] ValidatePortNames(string[] ports)
        {
            bool valid = true;
            List<string> output = new List<string>();
            foreach (string port in ports)
            {
                Match match = Regex.Match(port, @"COM\d*");
                if (match.Success)
                {
                    output.Add(match.Groups[0].Value);
                    valid &= match.Groups[0].Value.Equals(port);
                }
                else
                    valid &= false;
            }

            return output.ToArray();
        }
        /// <summary>
        /// HACK: Manipulate port name manually despite of available of local machine.
        /// </summary>
        /// <remarks>
        /// This is to handle exception scanario not encourage to use.
        /// </remarks>
        /// <returns></returns>
        private string[] GetPorts()
        {
            string[] ports = new string[20];
            for (int i = 0; i < 20; i++)
                ports[i] = "COM" + (i + 1).ToString();
            return ports;
        }
        #endregion

        #region INotifyPropertyChanged Members
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler handler = this.PropertyChanged;
            if ((handler != null))
            {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
    /// <summary>
    /// EventArgs used to send bytes recieved on serial port
    /// </summary>
    public class SerialDataEventArgs : EventArgs
    {
        public SerialDataEventArgs(byte[] dataInByteArray)
        {
            Data = dataInByteArray;
        }

        /// <summary>
        /// Byte array containing data from serial port
        /// </summary>
        public byte[] Data;
    }
}