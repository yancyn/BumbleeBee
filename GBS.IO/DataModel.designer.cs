// ------------------------------------------------------------------------------
//  <auto-generated>
//    Generated by Xsd2Code. Version 3.4.0.38967
//    <NameSpace>GBS.IO</NameSpace><Collection>ObservableCollection</Collection><codeType>CSharp</codeType><EnableDataBinding>True</EnableDataBinding><EnableLazyLoading>False</EnableLazyLoading><TrackingChangesEnable>False</TrackingChangesEnable><GenTrackingClasses>False</GenTrackingClasses><HidePrivateFieldInIDE>False</HidePrivateFieldInIDE><EnableSummaryComment>False</EnableSummaryComment><VirtualProp>False</VirtualProp><IncludeSerializeMethod>True</IncludeSerializeMethod><UseBaseClass>False</UseBaseClass><GenBaseClass>False</GenBaseClass><GenerateCloneMethod>False</GenerateCloneMethod><GenerateDataContracts>False</GenerateDataContracts><CodeBaseTag>Net35</CodeBaseTag><SerializeMethodName>Serialize</SerializeMethodName><DeserializeMethodName>Deserialize</DeserializeMethodName><SaveToFileMethodName>SaveToFile</SaveToFileMethodName><LoadFromFileMethodName>LoadFromFile</LoadFromFileMethodName><GenerateXMLAttributes>True</GenerateXMLAttributes><EnableEncoding>False</EnableEncoding><AutomaticProperties>False</AutomaticProperties><GenerateShouldSerialize>False</GenerateShouldSerialize><DisableDebug>False</DisableDebug><PropNameSpecified>Default</PropNameSpecified><Encoder>UTF8</Encoder><CustomUsings></CustomUsings><ExcludeIncludedTypes>False</ExcludeIncludedTypes><EnableInitializeFields>True</EnableInitializeFields>
//  </auto-generated>
// ------------------------------------------------------------------------------
namespace GBS.IO
{
    using System;
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System.Collections;
    using System.Xml.Schema;
    using System.ComponentModel;
    using System.IO;
    using System.Text;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;

    [Serializable]
    public partial class SerialCommand : System.ComponentModel.INotifyPropertyChanged
    {

        private string nameField;

        private bool enquiringField;

        private bool successField;

        private string groupIdField;

        private string parameterIdField;

        private Object parameterValueField;

        private string unitField;

        private List<KeyValuePair<int, string>> parameterOptionsField;

        private ParameterType parameterTypeField;

        private Object minValueField;

        private Object maxValueField;

        private static System.Xml.Serialization.XmlSerializer serializer;

        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                if ((this.nameField != null))
                {
                    if ((nameField.Equals(value) != true))
                    {
                        this.nameField = value;
                        this.OnPropertyChanged("Name");
                    }
                }
                else
                {
                    this.nameField = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        [XmlIgnore]
        public bool Enquiring
        {
            get { return this.enquiringField; }
            set
            {
                if ((this.enquiringField != null))
                {
                    if ((enquiringField.Equals(value) != true))
                    {
                        this.enquiringField = value;
                        this.OnPropertyChanged("Enquiring");
                    }
                }
                else
                {
                    this.enquiringField = value;
                    this.OnPropertyChanged("Enquiring");
                }
            }
        }
        [XmlIgnore]
        public bool Success { get { return this.successField; } }

        public string GroupId
        {
            get
            {
                return this.groupIdField;
            }
            set
            {
                if ((this.groupIdField != null))
                {
                    if ((groupIdField.Equals(value) != true))
                    {
                        this.groupIdField = value;
                        this.OnPropertyChanged("GroupId");
                    }
                }
                else
                {
                    this.groupIdField = value;
                    this.OnPropertyChanged("GroupId");
                }
            }
        }

        public string ParameterId
        {
            get
            {
                return this.parameterIdField;
            }
            set
            {
                if ((this.parameterIdField != null))
                {
                    if ((parameterIdField.Equals(value) != true))
                    {
                        this.parameterIdField = value;
                        this.OnPropertyChanged("ParameterId");
                    }
                }
                else
                {
                    this.parameterIdField = value;
                    this.OnPropertyChanged("ParameterId");
                }
            }
        }

        public Object ParameterValue
        {
            get
            {
                return this.parameterValueField;
            }
            set
            {
                if ((this.parameterValueField != null))
                {
                    if ((parameterValueField.Equals(value) != true))
                    {
                        this.parameterValueField = value;
                        this.OnPropertyChanged("ParameterValue");
                    }
                }
                else
                {
                    this.parameterValueField = value;
                    this.OnPropertyChanged("ParameterValue");
                }
            }
        }

        public string Unit
        {
            get
            {
                return this.unitField;
            }
            set
            {
                if ((this.unitField != null))
                {
                    if ((unitField.Equals(value) != true))
                    {
                        this.unitField = value;
                        this.OnPropertyChanged("Unit");
                    }
                }
                else
                {
                    this.unitField = value;
                    this.OnPropertyChanged("Unit");
                }
            }
        }

        public List<KeyValuePair<int, string>> ParameterOptions
        {
            get
            {
                return this.parameterOptionsField;
            }
            set
            {
                this.parameterOptionsField = value;
            }
        }

        public ParameterType ParameterType
        {
            get
            {
                return this.parameterTypeField;
            }
            set
            {
                if ((parameterTypeField.Equals(value) != true))
                {
                    this.parameterTypeField = value;
                    this.OnPropertyChanged("ParameterType");
                }
            }
        }

        public Object MinValue
        {
            get
            {
                return this.minValueField;
            }
            set
            {
                if ((this.minValueField != null))
                {
                    if ((minValueField.Equals(value) != true))
                    {
                        this.minValueField = value;
                        this.OnPropertyChanged("MinValue");
                    }
                }
                else
                {
                    this.minValueField = value;
                    this.OnPropertyChanged("MinValue");
                }
            }
        }

        public Object MaxValue
        {
            get
            {
                return this.maxValueField;
            }
            set
            {
                if ((this.maxValueField != null))
                {
                    if ((maxValueField.Equals(value) != true))
                    {
                        this.maxValueField = value;
                        this.OnPropertyChanged("MaxValue");
                    }
                }
                else
                {
                    this.maxValueField = value;
                    this.OnPropertyChanged("MaxValue");
                }
            }
        }

        private static System.Xml.Serialization.XmlSerializer Serializer
        {
            get
            {
                if ((serializer == null))
                {
                    serializer = new System.Xml.Serialization.XmlSerializer(typeof(SerialCommand));
                }
                return serializer;
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler handler = this.PropertyChanged;
            if ((handler != null))
            {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
                this.hasChanged |= true;
            }
        }
    }

    [Serializable]
    public partial class ParameterGroup : System.ComponentModel.INotifyPropertyChanged
    {

        private string headerField;

        private ObservableCollection<SerialCommand> commandsField;

        public string Header
        {
            get
            {
                return this.headerField;
            }
            set
            {
                if ((this.headerField != null))
                {
                    if ((headerField.Equals(value) != true))
                    {
                        this.headerField = value;
                        this.OnPropertyChanged("Header");
                    }
                }
                else
                {
                    this.headerField = value;
                    this.OnPropertyChanged("Header");
                }
            }
        }

        public ObservableCollection<SerialCommand> Commands
        {
            get
            {
                return this.commandsField;
            }
            set
            {
                if ((this.commandsField != null))
                {
                    if ((commandsField.Equals(value) != true))
                    {
                        this.commandsField = value;
                        this.OnPropertyChanged("Commands");
                    }
                }
                else
                {
                    this.commandsField = value;
                    this.OnPropertyChanged("Commands");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler handler = this.PropertyChanged;
            if ((handler != null))
            {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public enum ParameterType
    {

        /// <remarks/>
        Integer,

        /// <remarks/>
        Hex,

        /// <remarks/>
        String,
    }

    [Serializable]
    public partial class SerialCommander : System.ComponentModel.INotifyPropertyChanged
    {

        private string nameField;

        private string firmwareField;

        private string codeplugField;

        private string messageField;

        private string outputField;

        private ObservableCollection<ParameterGroup> commandGroupsField;

        private static System.Xml.Serialization.XmlSerializer serializer;

        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                if ((this.nameField != null))
                {
                    if ((nameField.Equals(value) != true))
                    {
                        this.nameField = value;
                        this.OnPropertyChanged("Name");
                    }
                }
                else
                {
                    this.nameField = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        public string Firmware
        {
            get
            {
                return this.firmwareField;
            }
            set
            {
                if ((this.firmwareField != null))
                {
                    if ((firmwareField.Equals(value) != true))
                    {
                        this.firmwareField = value;
                        this.OnPropertyChanged("Firmware");
                    }
                }
                else
                {
                    this.firmwareField = value;
                    this.OnPropertyChanged("Firmware");
                }
            }
        }

        public string Codeplug
        {
            get
            {
                return this.codeplugField;
            }
            set
            {
                if ((this.codeplugField != null))
                {
                    if ((codeplugField.Equals(value) != true))
                    {
                        this.codeplugField = value;
                        this.OnPropertyChanged("Codeplug");
                    }
                }
                else
                {
                    this.codeplugField = value;
                    this.OnPropertyChanged("Codeplug");
                }
            }
        }

        public string Message
        {
            get
            {
                return this.messageField;
            }
            set
            {
                if ((this.messageField != null))
                {
                    if ((messageField.Equals(value) != true))
                    {
                        this.messageField = value;
                        this.OnPropertyChanged("Message");
                    }
                }
                else
                {
                    this.messageField = value;
                    this.OnPropertyChanged("Message");
                }
            }
        }

        public string Output
        {
            get
            {
                return this.outputField;
            }
            set
            {
                if ((this.outputField != null))
                {
                    if ((outputField.Equals(value) != true))
                    {
                        this.outputField = value;
                        this.OnPropertyChanged("Output");
                    }
                }
                else
                {
                    this.outputField = value;
                    this.OnPropertyChanged("Output");
                }
            }
        }

        public ObservableCollection<ParameterGroup> CommandGroups
        {
            get
            {
                return this.commandGroupsField;
            }
            set
            {
                if ((this.commandGroupsField != null))
                {
                    if ((commandGroupsField.Equals(value) != true))
                    {
                        this.commandGroupsField = value;
                        this.OnPropertyChanged("CommandGroups");
                    }
                }
                else
                {
                    this.commandGroupsField = value;
                    this.OnPropertyChanged("CommandGroups");
                }
            }
        }

        private static System.Xml.Serialization.XmlSerializer Serializer
        {
            get
            {
                if ((serializer == null))
                {
                    serializer = new System.Xml.Serialization.XmlSerializer(typeof(SerialCommander));
                }
                return serializer;
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler handler = this.PropertyChanged;
            if ((handler != null))
            {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        #region Serialize/Deserialize
        /// <summary>
        /// Serializes current SerialCommander object into an XML document
        /// </summary>
        /// <returns>string XML value</returns>
        public virtual string Serialize()
        {
            System.IO.StreamReader streamReader = null;
            System.IO.MemoryStream memoryStream = null;
            try
            {
                memoryStream = new System.IO.MemoryStream();
                Serializer.Serialize(memoryStream, this);
                memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                streamReader = new System.IO.StreamReader(memoryStream);
                return streamReader.ReadToEnd();
            }
            finally
            {
                if ((streamReader != null))
                {
                    streamReader.Dispose();
                }
                if ((memoryStream != null))
                {
                    memoryStream.Dispose();
                }
            }
        }

        /// <summary>
        /// Deserializes workflow markup into an SerialCommander object
        /// </summary>
        /// <param name="xml">string workflow markup to deserialize</param>
        /// <param name="obj">Output SerialCommander object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool Deserialize(string xml, out SerialCommander obj, out System.Exception exception)
        {
            exception = null;
            obj = default(SerialCommander);
            try
            {
                obj = Deserialize(xml);
                return true;
            }
            catch (System.Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool Deserialize(string xml, out SerialCommander obj)
        {
            System.Exception exception = null;
            return Deserialize(xml, out obj, out exception);
        }

        public static SerialCommander Deserialize(string xml)
        {
            System.IO.StringReader stringReader = null;
            try
            {
                stringReader = new System.IO.StringReader(xml);
                return ((SerialCommander)(Serializer.Deserialize(System.Xml.XmlReader.Create(stringReader))));
            }
            finally
            {
                if ((stringReader != null))
                {
                    stringReader.Dispose();
                }
            }
        }

        /// <summary>
        /// Serializes current SerialCommander object into file
        /// </summary>
        /// <param name="fileName">full path of outupt xml file</param>
        /// <param name="exception">output Exception value if failed</param>
        /// <returns>true if can serialize and save into file; otherwise, false</returns>
        public virtual bool SaveToFile(string fileName, out System.Exception exception)
        {
            exception = null;
            try
            {
                SaveToFile(fileName);
                return true;
            }
            catch (System.Exception e)
            {
                exception = e;
                return false;
            }
        }

        public virtual void SaveToFile(string fileName)
        {
            System.IO.StreamWriter streamWriter = null;
            try
            {
                string xmlString = Serialize();
                System.IO.FileInfo xmlFile = new System.IO.FileInfo(fileName);
                streamWriter = xmlFile.CreateText();
                streamWriter.WriteLine(xmlString);
                streamWriter.Close();
            }
            finally
            {
                if ((streamWriter != null))
                {
                    streamWriter.Dispose();
                }
            }
        }

        /// <summary>
        /// Deserializes xml markup from file into an SerialCommander object
        /// </summary>
        /// <param name="fileName">string xml file to load and deserialize</param>
        /// <param name="obj">Output SerialCommander object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool LoadFromFile(string fileName, out SerialCommander obj, out System.Exception exception)
        {
            exception = null;
            obj = default(SerialCommander);
            try
            {
                obj = LoadFromFile(fileName);
                return true;
            }
            catch (System.Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool LoadFromFile(string fileName, out SerialCommander obj)
        {
            System.Exception exception = null;
            return LoadFromFile(fileName, out obj, out exception);
        }

        public static SerialCommander LoadFromFile(string fileName)
        {
            System.IO.FileStream file = null;
            System.IO.StreamReader sr = null;
            try
            {
                file = new System.IO.FileStream(fileName, FileMode.Open, FileAccess.Read);
                sr = new System.IO.StreamReader(file);
                string xmlString = sr.ReadToEnd();
                sr.Close();
                file.Close();
                return Deserialize(xmlString);
            }
            finally
            {
                if ((file != null))
                {
                    file.Dispose();
                }
                if ((sr != null))
                {
                    sr.Dispose();
                }
            }
        }
        #endregion
    }
}