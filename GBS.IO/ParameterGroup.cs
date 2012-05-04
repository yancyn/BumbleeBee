using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace GBS.IO
{
    public partial class ParameterGroup
    {
        /// <summary>
        /// ParameterGroup class constructor
        /// </summary>
        public ParameterGroup()
        {
            this.commandsField = new ObservableCollection<SerialCommand>();
        }
        /// <summary>
        /// Recommended constructor.
        /// </summary>
        /// <param name="header"></param>
        public ParameterGroup(string header)
        {
            this.commandsField = new ObservableCollection<SerialCommand>();
            this.headerField = header;
        }
    }
}