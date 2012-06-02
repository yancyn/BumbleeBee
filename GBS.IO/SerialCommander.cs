using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using System.Windows.Input;

namespace GBS.IO
{
    public partial class SerialCommander : IDisposable
    {
        #region Fields
        /// <summary>
        /// Interval waiting time when message keep writing.
        /// </summary>
        //const int ONE_SECOND = 1000;
        /// <summary>
        /// Interval waiting time when message keep retrieving.
        /// </summary>
        /// <remarks>
        /// This is required due to hardware enough time to response.
        /// </remarks>
        const int ONE_MOMENT = 200;
        /// <summary>
        /// Indicate which scenario to looking into when signal receive back from serial port.
        /// True it is writing data into serial port otherwise it is just retrieving value. 
        /// </summary>
        private bool isWriting;
        /// <summary>
        /// A stopper to prevent connected to a wrong device return no result
        /// so it is no point to continue retrieving.
        /// </summary>
        private bool neverSuccess;
        private SerialPortManager manager;
        public SerialPortManager Manager { get { return this.manager; } }
        /// <summary>
        /// current.serial - default configuration file to lookup at application directory.
        /// </summary>
        public const string DEFAULT_FILENAME = "current.serial";
        private SerialCommand currentCommand;

        private int lineNo;
        private int startIndex;

        private int startRetrieve;
        private int endRetrieve;

        /// <summary>
        /// Indicate the last processing index.
        /// This will be the next starting index again for retrieve.
        /// </summary>
        private int lastIndex;
        /// <summary>
        /// A counter for retrieving to a same com port connected.
        /// </summary>
        private int attempCounter;
        #endregion

        #region Properties
        private ConnectPortCommand connectPortCommand;
        /// <summary>
        /// Gets ICommand for start listening.
        /// </summary>
        public ConnectPortCommand ConnectPortCommand { get { return this.connectPortCommand; } }

        private StopPortCommand stopPortCommand;
        /// <summary>
        /// Gets ICommand for start listening.
        /// </summary>
        public StopPortCommand StopPortCommand { get { return this.stopPortCommand; } }

        private ApplyCommand applyCommand;
        public ApplyCommand ApplyCommand { get { return this.applyCommand; } }
        private RetrieveCommand retrieveCommand;
        public RetrieveCommand RetrieveCommand { get { return this.retrieveCommand; } }

        private ExportCommand exportCommand;
        public ExportCommand ExportCommand { get { return this.exportCommand; } }
        private ImportCommand importCommand;
        public ImportCommand ImportCommand { get { return this.importCommand; } }

        private LoadModeCommand loadModeCommand;
        public LoadModeCommand LoadModeCommand { get { return this.loadModeCommand; } }

        private UpgradeCommand upgradeCommand;
        public UpgradeCommand UpgradeCommand { get { return this.upgradeCommand; } }

        private ClearOutputCommand clearOutputCommand;
        public ClearOutputCommand ClearOutputCommand { get { return this.clearOutputCommand; } }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name"></param>
        public SerialCommander()
        {
            Initialize();
        }
        /// <summary>
        /// Recommended constructor.
        /// </summary>
        public SerialCommander(string name)
        {
            Initialize();
            this.nameField = name;
        }
        /// <summary>
        /// Deconstructor.
        /// </summary>
        ~SerialCommander()
        {
            Dispose();
        }
        #endregion

        #region Events
        void manager_NewSerialDataRecieved(object sender, SerialDataEventArgs e)
        {
            string output = Encoding.ASCII.GetString(e.Data);

            this.lineNo++;
            if (startRetrieve == 1) startIndex = lineNo;

            //retrieve scenario
            //if (output.Contains("REPLY"))
            this.outputsField.Add(output);
            //Match match = Regex.Match(output, @"REPLY\(\d{0,2}, \d{0,2}\): .*\r");
            //if (match.Success) this.outputsField.Add(output);

            this.outputField += output;// +"\n"; original output already contains feed line.
            OnPropertyChanged("Output");
        }
        /// <summary>
        /// Deprecated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void outputs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            while (this.outputsField.Count > 0)
            {
                string output = this.outputsField[0];
                //System.Diagnostics.Debug.WriteLine("outputs_CollectionChanged: "+output);
                string[] lines = output.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    Match match = Regex.Match(line, @"REPLY\(\d{0,2}, \d{0,2}\): .*\r");
                    if (!match.Success) continue;

                    string[] hold = new string[3];
                    MatchCollection matches = Regex.Matches(match.Groups[0].Value, @"\d{2}");
                    if (matches.Count < 2) continue;

                    hold[0] = matches[0].Groups[0].Value;
                    hold[1] = matches[1].Groups[0].Value;
                    match = Regex.Match(line, @": .*");
                    if (match.Success)
                    {
                        hold[2] = match.Groups[0].Value.Replace(":", string.Empty);
                        hold[2] = hold[2].Trim();
                    }

                    bool found = false;
                    #region Lookup correct command
                    if (this.firmwareField.Length == 0)
                    {
                        if (hold[0].Equals("05") && hold[1].Equals("02"))
                        {
                            this.firmwareField = hold[2];
                            OnPropertyChanged("Firmware");
                        }
                    }
                    if (this.codeplugField.Length == 0)
                    {
                        if (hold[0].Equals("05") && hold[1].Equals("03"))
                        {
                            this.codeplugField = hold[2];
                            OnPropertyChanged("Codeplug");
                        }
                    }
                    foreach (ParameterGroup group in this.commandGroupsField)
                    {
                        foreach (SerialCommand command in group.Commands)
                        {
                            if (command.GroupId.Equals(hold[0]) && command.ParameterId.Equals(hold[1]))
                            //&& currentCommand.GroupId.Equals(command.GroupId) && currentCommand.ParameterId.Equals(command.ParameterId))
                            {
                                //currentCommand.SetSuccess();
                                switch (command.ParameterType)
                                {
                                    case ParameterType.Integer:
                                        int result = 0;
                                        if (hold[2].ToString() == "0")
                                        {
                                            command.ParameterValue = 0;
                                            command.SetSuccess();
                                        }
                                        else
                                        {
                                            Int32.TryParse(hold[2].ToString(), out result);
                                            if (result > 0)
                                            {
                                                command.ParameterValue = result;
                                                command.SetSuccess();
                                            }
                                        }
                                        break;
                                    case ParameterType.String:
                                        command.ParameterValue = hold[2].ToString();
                                        command.SetSuccess();
                                        break;
                                    case ParameterType.Hex:
                                        //from 0xFFFF convert back to 65535
                                        command.ParameterValue = System.Convert.ToInt32(hold[2].ToString().ToLower(), 16);
                                        command.SetSuccess();
                                        break;
                                }

                                OnPropertyChanged("ParameterValue");
                                found = true;
                                break;
                            }
                        }

                        if (found) break;
                    }
                    #endregion
                }

                this.outputsField.RemoveAt(0);
            }

            //OnPropertyChanged("Success");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initial variables.
        /// </summary>
        protected void Initialize()
        {
            this.lineNo = 0;
            this.startIndex = 0;
            this.startRetrieve = 0;
            this.endRetrieve = 0;

            this.isWriting = false;
            this.neverSuccess = true;
            this.attempCounter = 0;
            this.lastIndex = 0;

            this.nameField = string.Empty;
            this.firmwareField = string.Empty;
            this.codeplugField = string.Empty;
            this.messageField = "Ready";
            this.outputField = string.Empty;
            this.outputsField = new ObservableCollection<string>();
            //this.outputsField.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(outputs_CollectionChanged);
            this.commandGroupsField = new ObservableCollection<ParameterGroup>();

            this.manager = new SerialPortManager();
            this.manager.NewSerialDataRecieved += new EventHandler<SerialDataEventArgs>(manager_NewSerialDataRecieved);

            this.connectPortCommand = new ConnectPortCommand(this);
            this.stopPortCommand = new StopPortCommand(this);
            this.retrieveCommand = new RetrieveCommand(this);
            this.applyCommand = new ApplyCommand(this);
            this.exportCommand = new ExportCommand(this);
            this.importCommand = new ImportCommand(this);
            this.loadModeCommand = new LoadModeCommand(this);
            this.upgradeCommand = new UpgradeCommand(this);
            this.clearOutputCommand = new ClearOutputCommand(this);
        }
        /// <summary>
        /// Deprecated.
        /// </summary>
        public void Connect()
        {
            SetMessage("Connecting to COM...");
            try
            {
                this.manager.StartListening();
            }
            catch (Exception ex)
            {
                Logger.Error(typeof(SerialCommander), ex);
                this.messageField = ex.Message;
                OnPropertyChanged("Message");

                this.manager.StopListening();
                //Disconnect();
                return;
            }
            finally
            {
                //Thread.Sleep(ONE_MOMENT);
                //SetMessage("Ready");
            }
        }
        /// <summary>
        /// Disconnect COM port.
        /// </summary>
        public void Disconnect()
        {
            SetMessage("Disconnected COM.");
            this.manager.StopListening();
            //if (thread != null) thread.Abort();
        }
        /// <summary>
        /// Apply changes.
        /// </summary>
        public void Apply()
        {
            //attempts = 0;
            if (!manager.IsOpen)
            {
                SetMessage("No port connected!");
                return;
            }

            this.isWriting = true;
            foreach (ParameterGroup group in this.commandGroupsField)
            {
                foreach (SerialCommand command in group.Commands)
                {
                    if (command.HasChanged)
                    {
                        SetMessage(string.Format("applying {0}: {1}", command.Name, command.ParameterValue));
                        command.SetEnquiring(true);

                        try
                        {
                            Write(command);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(typeof(SerialCommander), ex);
                            SetMessage(ex.Message);
                            break;
                        }

                        command.ResetState();
                        //give hardware device a rest and process time
                        Thread.Sleep(ONE_MOMENT);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieve data from connected device through com port.
        /// </summary>
        public void Retrieve()
        {
            startRetrieve++;
            //ensure it is only execute one
            //if (thread != null) thread.Abort();
            //threadStart = new ThreadStart(ProcessQueue);
            //thread = new Thread(threadStart);
            //thread.Start();
            //System.Diagnostics.Debug.WriteLine("thread started");

            this.isWriting = false;
            this.neverSuccess = true;
            int counter = 0;
            int totalFail = 1;
            //while (totalFail > 0)
            //{
            counter++;
            //if (counter > 2 && neverSuccess) break;

            totalFail = 0;
            //get version
            //if (this.firmwareField.Length == 0)
            //{
            //    totalFail++;
            SetMessage("Reading firmware...");
            SerialCommand command = new SerialCommand("Firmware Revision Number", ParameterType.String);
            command.GroupId = "05";
            command.ParameterId = "02";
            Read(command);
            Thread.Sleep(ONE_MOMENT);
            //}

            //if (this.codeplugField.Length == 0)
            //{
            totalFail++;
            SetMessage("Reading codeplug...");
            SerialCommand command2 = new SerialCommand("Codeplug Revision Number", ParameterType.String);
            command2.GroupId = "05";
            command2.ParameterId = "03";
            Read(command2);
            Thread.Sleep(ONE_MOMENT);
            //}

            //execute all commands
            foreach (ParameterGroup group in this.commandGroupsField)
            {
                foreach (SerialCommand cmd in group.Commands)
                {
                    //if (!cmd.Success)
                    //{
                    SetMessage("Reading " + cmd.Name + "...");
                    Read(cmd);
                    Thread.Sleep(ONE_MOMENT);
                    totalFail++;
                    //}
                }
            }
            //}//end loop

            //Thread.Sleep(5000);
            ProcessOutputsToLine();
            //ProcessOutputsCollection();//key: must process first before sending command to serial port
        }
        /// <summary>
        /// Process what we see in debug output.
        /// </summary>
        /// <remarks>
        /// todo: refactor for better performance.
        /// </remarks>
        private void ProcessOutputsToLine()
        {
            string cache = string.Empty;
            for (int i = lastIndex; i < this.outputsField.Count; i++)
                cache += this.outputsField[i];
            lastIndex = this.outputsField.Count - 1;

            string[] lines = cache.Split(new char[] { '\r', '\n' });
            foreach (string line in lines)
            {
                Match match = Regex.Match(line, @"\+REPLY\(\d{0,2}, \d{0,2}\): .*");
                if (match.Success)
                {
                    string[] hold = new string[3];
                    MatchCollection matches = Regex.Matches(match.Groups[0].Value, @"\d{2}");
                    if (matches.Count < 2) continue;

                    hold[0] = matches[0].Groups[0].Value;
                    hold[1] = matches[1].Groups[0].Value;
                    match = Regex.Match(line, @": .*");
                    if (match.Success)
                    {
                        hold[2] = match.Groups[0].Value.Replace(":", string.Empty);
                        hold[2] = hold[2].Trim();
                    }

                    LookupCommand(hold[0], hold[1], hold[2]);
                }
            }
        }
        /// <summary>
        /// Process all output we having from manager_NewSerialDataRecieved result.
        /// </summary>
        private void ProcessOutputsCollection()
        {
            System.Diagnostics.Debug.WriteLine("ProcessOutputsCollection");
            //for (int i = this.outputsField.Count - 1; i >= 0; i--)
            //{
            while (this.outputsField.Count > 0)
            {
                int i = this.outputsField.Count - 1;
                string output = this.outputsField[i];

                if (!this.outputField.Contains("REPLY"))
                {
                    this.outputsField.RemoveAt(i);
                    continue;
                }

                string[] lines = output.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    Match match = Regex.Match(line, @"REPLY\(\d{0,2}, \d{0,2}\): .*\r");
                    if (!match.Success) continue;

                    string[] hold = new string[3];
                    MatchCollection matches = Regex.Matches(match.Groups[0].Value, @"\d{2}");
                    if (matches.Count < 2) continue;

                    hold[0] = matches[0].Groups[0].Value;
                    hold[1] = matches[1].Groups[0].Value;
                    match = Regex.Match(line, @": .*");
                    if (match.Success)
                    {
                        hold[2] = match.Groups[0].Value.Replace(":", string.Empty);
                        hold[2] = hold[2].Trim();
                    }

                    LookupCommand(hold[0], hold[1], hold[2]);
                }

                this.outputsField.RemoveAt(i);
            }//end loop
        }
        /// <summary>
        /// Lookup whole SerialCommand collection to map value return.
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="paramId"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        private bool LookupCommand(string groupId, string paramId, string paramValue)
        {
            if (this.firmwareField.Length == 0)
            {
                if (groupId.Equals("05") && paramId.Equals("02"))
                {
                    this.firmwareField = paramValue;
                    OnPropertyChanged("Firmware");
                    neverSuccess &= false;
                    return true;
                }
            }
            if (this.codeplugField.Length == 0)
            {
                if (groupId.Equals("05") && paramId.Equals("03"))
                {
                    this.codeplugField = paramValue;
                    OnPropertyChanged("Codeplug");
                    neverSuccess &= false;
                    return true;
                }
            }

            foreach (ParameterGroup group in this.commandGroupsField)
            {
                foreach (SerialCommand command in group.Commands)
                {
                    if (command.GroupId.Equals(groupId) && command.ParameterId.Equals(paramId))
                    {
                        switch (command.ParameterType)
                        {
                            case ParameterType.Integer:
                                int result = 0;
                                if (paramValue == "0")
                                {
                                    command.ParameterValue = 0;
                                    command.SetSuccess();
                                }
                                else
                                {
                                    Int32.TryParse(paramValue, out result);
                                    if (result > 0)
                                    {
                                        command.ParameterValue = result;
                                        command.SetSuccess();
                                    }
                                }
                                break;
                            case ParameterType.String:
                                command.ParameterValue = paramValue;
                                command.SetSuccess();
                                break;
                            case ParameterType.Hex:
                                //from 0xFFFF convert back to 65535
                                command.ParameterValue = System.Convert.ToInt32(paramValue.ToLower(), 16);
                                command.SetSuccess();
                                break;
                        }

                        OnPropertyChanged("ParameterValue");
                        neverSuccess &= false;
                        return true;
                    }
                }
            }//end loop group

            return false;
        }

        /// <summary>
        /// Load layout with 0 or empty string.
        /// </summary>
        public void LoadSetting()
        {
            LoadSetting(DEFAULT_FILENAME);
        }
        public void LoadSetting(string fileName)
        {
            SetMessage("Load setting from " + fileName + "...");
            fileName = AppDomain.CurrentDomain.BaseDirectory + fileName;
            if (File.Exists(fileName))
            {
                SerialCommander commander = LoadFromFile(fileName);
                Clone(commander);
            }
            //commander.Dispose();
        }
        /// <summary>
        /// Export current all setting value to a datasheet for backup or furture use.
        /// </summary>
        public void ExportSetting(string fileName)
        {
            SetMessage("Export setting to file.");
            SaveToFile(fileName);
        }
        /// <summary>
        /// TODO: Upgrade to specified firmware version.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool Upgrade(string fileName)
        {
            SetMessage("Upgrading firmware...");

            SerialCommand command = new SerialCommand("Upgrading firmware", ParameterType.String);
            command.GroupId = "05";
            command.ParameterId = "00";
            command.ParameterValue = 0;
            command.SetEnquiring(true);

            try
            {
                Write(command);
                Thread.Sleep(2000);
                this.manager.Write("1");
                Disconnect();

                YModem ymodem = new YModem(manager.CurrentSerialSettings.PortName);
                ymodem.SendBinaryFile(fileName);
                Thread.Sleep(8000);

                Connect();
                this.manager.Write("2");

                command.ResetState();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(typeof(SerialCommander), ex);
                SetMessage(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Clear all output message to debug window in interface.
        /// </summary>
        public void ClearOutput()
        {
            this.outputField = string.Empty;
            this.outputsField.Clear();
            this.lastIndex = 0;
            OnPropertyChanged("Output");
        }
        #endregion

        #region Functions
        /// <summary>
        /// Write data to serial port and read return value.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public void Read(SerialCommand command)
        {
            if (command == null) return;

            command.Enquiring = true;
            string data = string.Format("$CMD,R,{0},{1}*CS#\r\n", command.GroupId, command.ParameterId);

            System.Diagnostics.Debug.WriteLine("Writing " + data);
            if (this.manager.IsOpen) this.manager.Write(data);
            //System.Threading.Thread.Sleep(2000);

            //command.ParameterValue = result;
            //OnPropertyChanged("ParameterValue");
            //return successOnce;
        }
        /// <summary>
        /// Write data to serial port and confirm success.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public void Write(SerialCommand command)
        {
            if (command == null) return;

            command.Enquiring = true;

            string valueToSend = command.ParameterValue.ToString();
            if (command.ParameterValue is bool)
            {
                bool value = (bool)command.ParameterValue;
                valueToSend = (value) ? "1" : "0";
            }
            else if (command.ParameterValue is KeyValuePair<int, string>)
            {
                KeyValuePair<int, string> hold = (KeyValuePair<int, string>)command.ParameterValue;
                valueToSend = hold.Key.ToString();
            }

            string data = string.Format("$CMD,W,{0},{1},{2}*CS#\r\n", command.GroupId, command.ParameterId, valueToSend);
            System.Diagnostics.Debug.WriteLine("Writing " + data);
            if (this.manager.IsOpen) this.manager.Write(data);
        }
        /// <summary>
        /// Clone all serial command attribute with 0 parameter value.
        /// </summary>
        /// <param name="commander"></param>
        private void Clone(SerialCommander commander)
        {
            this.nameField = commander.Name;

            this.codeplugField = string.Empty;
            OnPropertyChanged("CodePlug");

            this.firmwareField = string.Empty;
            OnPropertyChanged("FirmWare");

            this.commandGroupsField.Clear();
            foreach (ParameterGroup group in commander.CommandGroups)
            {
                ParameterGroup g = new ParameterGroup(group.Header);
                foreach (SerialCommand command in group.Commands)
                {
                    SerialCommand cmd = new SerialCommand(command.Name, command.ParameterType);
                    cmd.GroupId = command.GroupId;
                    cmd.ParameterId = command.ParameterId;
                    cmd.ParameterOptions = command.ParameterOptions;
                    cmd.MinValue = command.MinValue;
                    cmd.MaxValue = command.MaxValue;
                    cmd.Unit = command.Unit;

                    //set default parametervalue and corrected minValue after deserialized
                    switch (cmd.ParameterType)
                    {
                        case ParameterType.Integer:
                            cmd.ParameterValue = 0;
                            //if (cmd.MinValue.GetType() != typeof(int))
                            //    cmd.MinValue = 0;
                            //if (cmd.MinValue.GetType() != typeof(Int32))
                            //    cmd.MinValue = 0;
                            break;
                        case ParameterType.String:
                            cmd.ParameterValue = string.Empty;
                            //if (cmd.MinValue.GetType() != typeof(string))
                            //    cmd.MinValue = string.Empty;
                            //if (cmd.MinValue.GetType() != typeof(String))
                            //    cmd.MinValue = string.Empty;
                            break;
                        case ParameterType.Hex:
                            //default hex value
                            cmd.ParameterValue = 0;// 0x0000;
                            break;
                    }

                    g.Commands.Add(cmd);
                }
                this.commandGroupsField.Add(g);
            }
        }
        /// <summary>
        /// TODO: Ensure all serial command update correctly.
        /// </summary>
        /// <param name="commander"></param>
        private void Copy(SerialCommander commander)
        {
            if (commander.CommandGroups.Count != this.CommandGroups.Count) return;
            for (int i = 0; i < commander.CommandGroups.Count; i++)
            {
                if (commander.CommandGroups[i].Commands.Count != this.CommandGroups[i].Commands.Count) break;
                for (int j = 0; j < commander.CommandGroups[i].Commands.Count; j++)
                    this.CommandGroups[i].Commands[j].ParameterValue = commander.CommandGroups[i].Commands[j].ParameterValue;
            }
        }
        /// <summary>
        /// Import setting ready for apply.
        /// TODO: ensure it is a same parameter set.
        /// </summary>
        public void ImportSetting(string fileName)
        {
            SetMessage("Import setting...");
            SerialCommander commander = LoadFromFile(fileName);
            Copy(commander);
            //commander.Dispose();
        }
        public void Dispose()
        {
            //if (thread != null)
            //    thread.Abort();
            if (manager != null)
                manager.Dispose();
        }
        /// <summary>
        /// Set value into message property and notify UI.
        /// </summary>
        /// <param name="message"></param>
        private void SetMessage(string message)
        {
            this.messageField = message;
            System.Diagnostics.Debug.WriteLine(this.messageField);
            OnPropertyChanged("Message");

            //System.Threading.Thread.Sleep(5000);
            //this.messageField = "Ready";
            //OnPropertyChanged("Message");
        }
        /// <summary>
        /// Deprecated: Thread to process output collection.
        /// </summary>
        private void ProcessQueue()
        {
            while (true)
            {
                //System.Diagnostics.Debug.WriteLine("ProcessQueue");
                lock (this.outputField)
                {
                    while (this.outputsField.Count > 0)
                    {
                        string output = this.outputsField[0];
                        //System.Diagnostics.Debug.WriteLine("ProcessQueue(" + this.outputsField.Count + ")");
                        string[] lines = output.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string line in lines)
                        {
                            Match match = Regex.Match(line, @"REPLY\(\d{0,2}, \d{0,2}\): .*\r");
                            if (!match.Success) continue;

                            string[] hold = new string[3];
                            MatchCollection matches = Regex.Matches(match.Groups[0].Value, @"\d{2}");
                            if (matches.Count < 2) continue;

                            hold[0] = matches[0].Groups[0].Value;
                            hold[1] = matches[1].Groups[0].Value;
                            match = Regex.Match(line, @": .*");
                            if (match.Success)
                            {
                                hold[2] = match.Groups[0].Value.Replace(":", string.Empty);
                                hold[2] = hold[2].Trim();
                            }

                            bool found = false;
                            #region Lookup correct command
                            if (this.firmwareField.Length == 0)
                            {
                                if (hold[0].Equals("05") && hold[1].Equals("02"))
                                {
                                    this.firmwareField = hold[2];
                                    OnPropertyChanged("Firmware");
                                }
                            }
                            if (this.codeplugField.Length == 0)
                            {
                                if (hold[0].Equals("05") && hold[1].Equals("03"))
                                {
                                    this.codeplugField = hold[2];
                                    OnPropertyChanged("Codeplug");
                                }
                            }
                            foreach (ParameterGroup group in this.commandGroupsField)
                            {
                                foreach (SerialCommand command in group.Commands)
                                {
                                    if (command.GroupId.Equals(hold[0]) && command.ParameterId.Equals(hold[1]))
                                    //&& currentCommand.GroupId.Equals(command.GroupId) && currentCommand.ParameterId.Equals(command.ParameterId))
                                    {
                                        //currentCommand.SetSuccess();
                                        switch (command.ParameterType)
                                        {
                                            case ParameterType.Integer:
                                                int result = 0;
                                                if (hold[2].ToString() == "0")
                                                {
                                                    command.ParameterValue = 0;
                                                    command.SetSuccess();
                                                }
                                                else
                                                {
                                                    Int32.TryParse(hold[2].ToString(), out result);
                                                    if (result > 0)
                                                    {
                                                        command.ParameterValue = result;
                                                        command.SetSuccess();
                                                    }
                                                }
                                                break;
                                            case ParameterType.String:
                                                command.ParameterValue = hold[2].ToString();
                                                command.SetSuccess();
                                                break;
                                            case ParameterType.Hex:
                                                //from 0xFFFF convert back to 65535
                                                command.ParameterValue = System.Convert.ToInt32(hold[2].ToString().ToLower(), 16);
                                                command.SetSuccess();
                                                break;
                                        }

                                        OnPropertyChanged("ParameterValue");
                                        found = true;
                                        break;
                                    }
                                }

                                if (found) break;
                            }
                            #endregion
                        }

                        if (this.outputsField.Count > 0)
                            this.outputsField.RemoveAt(0);
                    }
                }
            }
        }
        public static string[] GetDefaultCurrentFileNames()
        {
            string[] fileNames = new string[3];
            fileNames[0] = DEFAULT_FILENAME;

            //string prefix = DEFAULT_FILENAME.Substring(0, DEFAULT_FILENAME.IndexOf('.'));
            string fileName = System.IO.Path.GetFileNameWithoutExtension(DEFAULT_FILENAME);
            string extension = System.IO.Path.GetExtension(DEFAULT_FILENAME);
            for (int i = 1; i < fileNames.Length; i++)
                fileNames[i] = fileName + i + extension;

            return fileNames;
        }
        #endregion
    }
    /// <summary>
    /// A command call SerialPortManager.StartListening method.
    /// </summary>
    public class ConnectPortCommand : ICommand
    {
        private SerialCommander manager;
        public ConnectPortCommand(SerialCommander manager)
        {
            this.manager = manager;
        }
        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            this.manager.Connect();
        }
        #endregion
    }
    /// <summary>
    /// A command call SerialPortManager.StartListening method.
    /// </summary>
    public class StopPortCommand : ICommand
    {
        private SerialCommander manager;
        public StopPortCommand(SerialCommander manager)
        {
            this.manager = manager;
        }
        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            this.manager.Disconnect();
        }
        #endregion
    }

    public class ApplyCommand : ICommand
    {
        private SerialCommander manager;
        public ApplyCommand(SerialCommander manager)
        {
            this.manager = manager;
        }
        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            manager.Apply();
        }
        #endregion
    }

    public class RetrieveCommand : ICommand
    {
        private SerialCommander manager;
        public RetrieveCommand(SerialCommander manager)
        {
            this.manager = manager;
        }
        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            this.manager.Retrieve();
        }
        #endregion
    }

    public class ExportCommand : ICommand
    {
        private SerialCommander manager;
        public ExportCommand(SerialCommander manager)
        {
            this.manager = manager;
        }
        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            this.manager.ExportSetting(parameter.ToString());
        }
        #endregion
    }

    public class ImportCommand : ICommand
    {
        private SerialCommander manager;
        public ImportCommand(SerialCommander manager)
        {
            this.manager = manager;
        }
        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            this.manager.ImportSetting(parameter.ToString());
        }
        #endregion
    }
    /// <summary>
    /// Switching different mode like PRX Modem, Page, or PTX unit.
    /// </summary>
    public class LoadModeCommand : ICommand
    {
        private SerialCommander manager;
        public LoadModeCommand(SerialCommander manager)
        {
            this.manager = manager;
        }
        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            this.manager.LoadSetting(parameter.ToString());
        }
        #endregion
    }
    public class UpgradeCommand : ICommand
    {
        private SerialCommander manager;
        public UpgradeCommand(SerialCommander manager)
        {
            this.manager = manager;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            this.manager.Upgrade(parameter.ToString());
        }
    }
    public class ClearOutputCommand : ICommand
    {
        private SerialCommander manager;
        public ClearOutputCommand(SerialCommander manager)
        { this.manager = manager; }
        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            this.manager.ClearOutput();
        }
        #endregion
    }

    /// <summary>
    /// For serialization use only.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <seealso>http://stackoverflow.com/questions/83232/is-there-a-serializable-generic-key-value-pair-class-in-net</seealso>
    [Serializable]
    public struct KeyValuePair<Int32, String>
    {
        private int key;
        public int Key { get { return this.key; } set { this.key = value; } }
        private string value;
        public string Value { get { return this.value; } set { this.value = value; } }
        public KeyValuePair(int key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }
}