using System.Xml.Schema;
using System.Xml.Serialization;
using XmlSerializer.Models;

namespace UnWin.Models;

public class AutoLogon
{
    [XmlElement(ElementName = "Password")]
    public Password Password { get; set; }

    [XmlElement(ElementName = "Enabled")]
    public bool Enabled { get; set; }

    [XmlElement(ElementName = "LogonCount")]
    public int LogonCount { get; set; }

    [XmlAttribute(Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State", AttributeName = "action", Form = XmlSchemaForm.Qualified)]
    public string Action { get; set; }

    [XmlElement(ElementName = "Username")]
    public string Username { get; set; }
}