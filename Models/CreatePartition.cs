using System.Xml.Schema;
using System.Xml.Serialization;

namespace XmlSerializer.Models;

public class CreatePartition
{
    [XmlAttribute(Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State", AttributeName = "action", Form = XmlSchemaForm.Qualified)]
    public string Action { get; set; }

    [XmlElement(ElementName = "Order")]
    public int Order { get; set; }

    [XmlElement(ElementName = "Size")]
    public int Size { get; set; }

    [XmlElement(ElementName = "Type")]
    public string Type { get; set; }
}