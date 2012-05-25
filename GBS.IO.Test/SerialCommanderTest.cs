using GBS.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace GBS.IO.Test
{
    /// <summary>
    /// This is a test class for SerialCommanderTest and is intended
    /// to contain all SerialCommanderTest Unit Tests
    /// </summary>
    [TestClass()]
    public class SerialCommanderTest
    {
        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for SaveToFile
        ///</summary>
        [TestMethod()]
        public void SaveToFileTest()
        {
            SerialCommander target = new SerialCommander("BumbleBee");

            ParameterGroup group = new ParameterGroup("Initialization LED");
            group.Commands.Add(new SerialCommand("LED Blinking On Period", 100, 65535, "ms"));
            group.Commands.Add(new SerialCommand("LED Blinking Off Period", 100, 65535, "ms"));
            group.Commands.Add(new SerialCommand("Number of LED Blinking", 10, 255));
            group.Commands.Add(new SerialCommand("Fail LED On Period", 20, 65535, "sec"));

            ParameterGroup group2 = new ParameterGroup();
            group2.Commands.Add(new SerialCommand("Startup Delay", 10, 65535, "sec"));
            group2.Commands.Add(new SerialCommand("Temperature Sampling Period", 10, 65535, "sec"));

            ParameterGroup group3 = new ParameterGroup();
            group3.Commands.Add(new SerialCommand("Normal Reporting Period", 10, 65535, "sec"));
            group3.Commands.Add(new SerialCommand("Violation Reporting Period", 5, 65535, "sec"));
            group3.Commands.Add(new SerialCommand("Tag ID", 305419896, ParameterType.Hex));
            group3.Commands.Add(new SerialCommand("Group ID", 39321, ParameterType.Hex));

            ParameterGroup group4 = new ParameterGroup("RF");
            group4.Commands.Add(new SerialCommand("Maximum of RF Retries", 3, 255));
            group4.Commands.Add(new SerialCommand("RF Retry Delay", 500, 655353, "ms"));
            group4.Commands.Add(new SerialCommand("RF Maximum Wait ACK Period", 60, 255, "ms"));

            SerialCommand cmd = new SerialCommand("RF TX Output Power", 4);
            cmd.ParameterOptions.Add(new KeyValuePair<int, string>(0, "-30 dBm"));
            cmd.ParameterOptions.Add(new KeyValuePair<int, string>(1, "-20 dBm"));
            cmd.ParameterOptions.Add(new KeyValuePair<int, string>(2, "-15 dBm"));
            cmd.ParameterOptions.Add(new KeyValuePair<int, string>(3, "-10 dBm"));
            cmd.ParameterOptions.Add(new KeyValuePair<int, string>(4, "0 dBm"));
            cmd.ParameterOptions.Add(new KeyValuePair<int, string>(5, "+5 dBm"));
            cmd.ParameterOptions.Add(new KeyValuePair<int, string>(6, "+7 dBm"));
            cmd.ParameterOptions.Add(new KeyValuePair<int, string>(7, "+10 dBm"));
            group4.Commands.Add(cmd);

            group4.Commands.Add(new SerialCommand("RF TX/RX LED Blinking On Period", 50, 655353, "ms"));
            group4.Commands.Add(new SerialCommand("RF TX/RX LED Blinking Off Period", 50, 655353, "ms"));
            group4.Commands.Add(new SerialCommand("RF TX/RX Number of LED Blinking", 2, 255));
            group4.Commands.Add(new SerialCommand("RF Device Address", 136, 255));

            ParameterGroup group5 = new ParameterGroup();
            group5.Commands.Add(new SerialCommand("Maximum Wait Period To Process PC Command", 15, 255, "sec"));

            SerialCommand cmd2 = new SerialCommand("Power Saving Mode", 1);
            cmd2.ParameterOptions.Add(new KeyValuePair<int, string>(0, "DISABLE"));
            cmd2.ParameterOptions.Add(new KeyValuePair<int, string>(1, "ENABLE"));
            group5.Commands.Add(cmd2);

            target.CommandGroups.Add(group);
            target.CommandGroups.Add(group2);
            target.CommandGroups.Add(group3);
            target.CommandGroups.Add(group4);
            target.CommandGroups.Add(group5);


            string fileName = "current.serial";
            target.SaveToFile(fileName);
            Assert.IsTrue(System.IO.File.Exists(fileName));
        }
        /// <summary>
        ///A test for LoadSetting
        ///</summary>
        [TestMethod()]
        public void LoadSettingTest()
        {
            SerialCommander target = new SerialCommander("BumbleBee");
            target.LoadSetting();
            Assert.IsTrue(target.CommandGroups.Count > 0);
        }
        [TestMethod()]
        public void GetVersionTest()
        {
            SerialCommander target = new SerialCommander();
            target.Manager.CurrentSerialSettings.PortName = "COM3";
            target.Manager.CurrentSerialSettings.BaudRate = 115200;
            target.Manager.CurrentSerialSettings.Parity = System.IO.Ports.Parity.None;
            target.Manager.CurrentSerialSettings.DataBits = 8;
            target.Manager.CurrentSerialSettings.StopBits = System.IO.Ports.StopBits.One;
            target.Manager.StartListening();

            //SerialCommand command = new SerialCommand("Firmware Revision Number", ParameterType.String);
            //command.GroupId = "05";
            //command.ParameterId = "02";
            //System.Threading.Thread.Sleep(2000);
            //Assert.IsTrue(target.Read(command));

            //target.LoadSetting();
            target.Retrieve();

            System.Threading.Thread.Sleep(3000);
            System.Diagnostics.Debug.WriteLine(target.Firmware);
            Assert.IsTrue(target.Firmware.Length > 0);
        }

        [TestMethod()]
        public void TrimTest()
        {
            string expected = "D00.03";
            string actual = string.Empty;

            string target = "+REPLY(05, 02): D00.03\r\n";
            string pattern = "+REPLY(05, 02):";
            if (target.Contains(pattern))
            {
                string[] pieces = target.Split(new char[] { '\r', '\n' });
                actual = pieces[0].Replace(pattern, string.Empty).Trim();
                System.Diagnostics.Debug.WriteLine("result:" + actual);
            }

            Assert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public void TryParseCorrectStringTest()
        {
            int expected = 23;
            int actual = 0;
            string target = "23";
            Int32.TryParse(target, out actual);
            Assert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public void TryParseErrorStringTest()
        {
            int expected = 23;
            int actual = 23;
            string target = "abc";
            Int32.TryParse(target, out actual);
            Assert.AreNotEqual(expected, actual);
        }
        /// <summary>
        /// Originally true however once has been assign false then the value forever will be false.
        /// </summary>
        [TestMethod()]
        public void AndFalseTest()
        {
            bool expected = false;
            bool actual = true;
            bool[] targets = new bool[3] { true, false, true };
            foreach (bool target in targets)
                actual &= target;

            Assert.AreEqual(expected, actual);
        }
        /// <summary>
        /// Originally false however once assigned true then became true forever.
        /// </summary>
        [TestMethod()]
        public void OrTrueTest()
        {
            bool expected = true;
            bool actual = false;
            bool[] targets = new bool[3] { false, true, false };
            foreach (bool target in targets)
                actual |= target;

            Assert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public void MatchDigitsTest()
        {
            string target = "+REPLY(00, 03): 100";
            string pattern = @"\d+";

            string[] expected = new string[] { "00", "03", "100" };
            string[] actual = new string[3];
            MatchCollection matches = Regex.Matches(target, pattern);
            for (int i = 0; i < Math.Min(3, matches.Count); i++)
                actual[i] = matches[i].Groups[0].Value;

            CollectionAssert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public void MatchComNumericResultTest()
        {
            string expected = "+REPLY(00, 03): 100";
            string actual = string.Empty;

            string target = "+REPLY(00, 03): 100\r\n";
            string pattern = @"\+REPLY\(\d{0,2}, \d{0,2}\): \d*";
            Match match = Regex.Match(target, pattern);
            if (match.Success)
                actual = match.Groups[0].Value;

            Assert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public void MatchComStringResultTest()
        {
            string expected = "+REPLY(05, 03): D01.01";
            string actual = string.Empty;

            string target = "+REPLY(05, 03): D01.01\r\nOK\r\n\r\n";
            string pattern = @"\+REPLY\(\d{0,2}, \d{0,2}\): .*\r\n";
            Match match = Regex.Match(target, pattern);
            if (match.Success)
                actual = match.Groups[0].Value.Replace("\r\n", string.Empty);

            Assert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public void MatchComAnyResultTest()
        {
            string expected = "+REPLY(00, 03): 100";
            string actual = string.Empty;

            string target = "+REPLY(00, 03): 100\r\n";
            string pattern = @"\+REPLY\(\d{0,2}, \d{0,2}\): .*\r\n";
            Match match = Regex.Match(target, pattern);
            if (match.Success)
                actual = match.Groups[0].Value.Replace("\r\n", string.Empty);
            Assert.AreEqual(expected, actual);

            //fail scenario
            //target = @"3\r\nOK\r\n\r\n+REPLY(05, 03): D01.01\r\nOK\r\n\r\n\r\r\nERROR: Failed to read temperature level (error = 2)\r\r\n";
            //expected = "+REPLY(05, 03): D01.01";
            //match = Regex.Match(target, pattern,RegexOptions.Multiline);
            //if (match.Success)
            //    actual = match.Groups[0].Value.Replace("\r\n", string.Empty);
            //else
            //    actual = string.Empty;
            //Assert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public void MatchComMultilineResultTest()
        {
            //fail scenario
            string target = @"3\r\nOK\r\n\r\n+REPLY(05, 03): D01.01\r\nOK\r\n\r\n\r\r\nERROR: Failed to read temperature level (error = 2)\r\r\n";
            string expected = "+REPLY(05, 03): D01.01";
            string actual = string.Empty;

            string[] lines = target.Split(new string[] { @"\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string pattern = @"\+REPLY\(\d{0,2}, \d{0,2}\): .*";
                Match match = Regex.Match(line, pattern);
                if (match.Success)
                {
                    actual = match.Groups[0].Value.Replace("\r\n", string.Empty);
                    break;
                }
            }
            Assert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public void TailComStringResultTest()
        {
            string expected = "D01.01";
            string actual = string.Empty;

            string target = "+REPLY(05, 03): D01.01\r\nOK\r\n\r\n";
            string pattern = @": .*\r\n";
            Match match = Regex.Match(target, pattern);
            if (match.Success)
            {
                actual = match.Groups[0].Value.Replace("\r\n", string.Empty);
                actual = match.Groups[0].Value.Replace(":", string.Empty);
                actual = actual.Trim();
            }

            Assert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public void ValidateWrongPortNameTest()
        {
            string pattern = @"COM\d*";
            string target = "COM1";
            bool expected = true;
            bool actual = false;
            Match match = Regex.Match(target, pattern);
            if (match.Success)
                actual = (match.Groups[0].Value.Equals(target));
            else
                actual = false;
            Assert.AreEqual(expected, actual);

            target = "COM13";
            expected = true;
            match = Regex.Match(target, pattern);
            if (match.Success)
                actual = (match.Groups[0].Value.Equals(target));
            else
                actual = false;
            Assert.AreEqual(expected, actual);

            target = "COM8!";
            expected = false;
            match = Regex.Match(target, pattern);
            if (match.Success)
                actual = (match.Groups[0].Value.Equals(target));
            else
                actual = false;
            Assert.AreEqual(expected, actual);

            target = "COM18?";
            expected = false;
            match = Regex.Match(target, pattern);
            if (match.Success)
                actual = (match.Groups[0].Value.Equals(target));
            else
                actual = false;
            Assert.AreEqual(expected, actual);

            target = "COM20";
            expected = true;
            match = Regex.Match(target, pattern);
            if (match.Success)
                actual = (match.Groups[0].Value.Equals(target));
            else
                actual = false;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void IntToHexTest()
        {
            int target = 15;
            string expected = "0x000f";
            string actual = String.Format("0x{0:x4}", target);//"0x{0:x8}"
            Assert.AreEqual(expected, actual);

            target = 65535;
            expected = "0xFFFF";
            actual = String.Format("0x{0:x4}", target);
            Assert.AreEqual(expected.ToUpper(), actual.ToUpper());
        }
        [TestMethod()]
        public void HexToIntTest()
        {
            string target = "0x0000";
            int expected = 0;
            int actual = Convert.ToInt32(target.ToLower(), 16);
            Assert.AreEqual(expected, actual);

            target = "0x00000000";
            expected = 0;
            actual = Convert.ToInt32(target.ToLower(), 16);
            Assert.AreEqual(expected, actual);

            target = "0xFFFF";
            expected = 65535;
            actual = Convert.ToInt32(target.ToLower(), 16);
            Assert.AreEqual(expected, actual);

            target = "0x0000FFFF";
            expected = 65535;
            actual = Convert.ToInt32(target.ToLower(), 16);
            Assert.AreEqual(expected, actual);

            target = "0xFFFFFFFF";
            expected = -1;
            actual = Convert.ToInt32(target.ToLower(), 16);
            Assert.AreEqual(expected, actual);



            target = "0x9999";
            expected = 39321;
            actual = Convert.ToInt32(target.ToLower(), 16);
            Assert.AreEqual(expected, actual);

            target = "0x12345678";
            expected = 305419896;
            actual = Convert.ToInt32(target.ToLower(), 16);
            Assert.AreEqual(expected, actual);
        }

        //[TestMethod()]
        public void UpgradeTest()
        {
            YModem ymodem = new YModem("COM3");
            ymodem.SendBinaryFile(@"C:\Users\yeang-shing.then\Desktop\cut.svg.plt");
        }

        [TestMethod()]
        public void OutputsCollectionChangedTest()
        {
            int counter = 0;
            ObservableCollection<string> targets = new ObservableCollection<string> { "a", "b", "c", "d", "e", "f" };
            while (targets.Count > 0)
            {
                counter++;
                System.Diagnostics.Debug.WriteLine(counter);
                targets.RemoveAt(0);
            }

            System.Diagnostics.Debug.WriteLine("total loops:" + counter);
        }
    }
}