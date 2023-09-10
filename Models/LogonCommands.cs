using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace UnWin.Models
{
    public class LogonCommands
    {
        [XmlElement(ElementName = "AsynchronousCommand")]
        public List<AsynchronousCommand> AsynchronousCommand { get; set; }
    }
}
