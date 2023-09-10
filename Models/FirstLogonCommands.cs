using System.Collections.Generic;
using System.Xml.Serialization;

namespace UnWin.Models;

public class FirstLogonCommands
{
    [XmlElement(ElementName = "SynchronousCommand")]
    public List<SynchronousCommand> SynchronousCommand { get; set; }
}