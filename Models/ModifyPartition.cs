using System.Xml.Schema;
using System.Xml.Serialization;

namespace XmlSerializer.Models;

public class ModifyPartition
{
    [XmlAttribute(Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State", AttributeName = "action", Form = XmlSchemaForm.Qualified)]
    public string Action { get; set; }

    [XmlElement(ElementName = "Format")]
    public string Format { get; set; }

    [XmlElement(ElementName = "Label")]
    public string Label { get; set; }

    [XmlElement(ElementName = "Order")]
    public int Order { get; set; }

    [XmlElement(ElementName = "PartitionID")]
    public int PartitionID { get; set; }

    [XmlElement(ElementName = "TypeID")]
    public string TypeID { get; set; }
}