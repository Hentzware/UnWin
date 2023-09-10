using System.Xml.Schema;
using System.Xml.Serialization;

namespace XmlSerializer.Models;

public class LocalAccount
{
    [XmlAttribute(Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State", AttributeName = "action", Form = XmlSchemaForm.Qualified)]
    public string Action { get; set; }

    [XmlElement(ElementName = "Password")]
    public Password Password { get; set; }

    [XmlElement(ElementName = "DisplayName")]
    public string DisplayName { get; set; }

    [XmlElement(ElementName = "Group")]
    public string Group { get; set; }

    [XmlElement(ElementName = "Name")]
    public string Name { get; set; }
}