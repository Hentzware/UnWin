using System.Xml.Serialization;

namespace UnWin.Models;

public class LogonCommand
{
    [XmlElement(ElementName = "RequiresUserInput")]
    public bool RequiresUserInput { get; set; }

    [XmlElement(ElementName = "Order")]
    public int Order { get; set; }

    [XmlElement(ElementName = "CommandLine")]
    public string Command { get; set; }
}