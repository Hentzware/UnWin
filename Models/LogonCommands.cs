using System.Collections.Generic;
using System.Xml.Serialization;

namespace UnWin.Models;

public class LogonCommands
{
    [XmlElement(ElementName = "AsynchronousCommand")]
    public List<AsynchronousCommand> AsynchronousCommand { get; set; }
}