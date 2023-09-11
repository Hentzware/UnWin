using System.Xml.Schema;
using System.Xml.Serialization;

namespace UnWin.Models;

public class MetaData
{
    [XmlElement(ElementName = "Key")]
    public string Key { get; set; }

    [XmlElement(ElementName = "Value")]
    public int Value { get; set; }

    [XmlAttribute(Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State", AttributeName = "action", Form = XmlSchemaForm.Qualified)]
    public string Action { get; set; }
}