using System.Xml.Schema;
using System.Xml.Serialization;

namespace UnWin.Models;

public class SynchronousCommand
{
    [XmlElement(ElementName = "CommandLine")]
    public string CommandLine { get; set; }

    [XmlElement(ElementName = "Order")]
    public int Order { get; set; }

    [XmlElement(ElementName = "RequiresUserInput")]
    public bool RequiresUserInput { get; set; }

    [XmlAttribute(Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State", AttributeName = "action", Form = XmlSchemaForm.Qualified)]
    public string Action { get; set; }
}