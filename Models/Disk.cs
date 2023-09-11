using System.Xml.Schema;
using System.Xml.Serialization;

namespace XmlSerializer.Models;

public class Disk
{
    [XmlElement(ElementName = "CreatePartitions")]
    public CreatePartitions? CreatePartitions { get; set; }

    [XmlElement(ElementName = "ModifyPartitions")]
    public ModifyPartitions? ModifyPartitions { get; set; }

    [XmlElement(ElementName = "DiskID")]
    public int DiskId { get; set; }

    [XmlElement(ElementName = "WillWipeDisk")]
    public bool WillWipeDisk { get; set; }


    [XmlAttribute(Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State", AttributeName = "action", Form = XmlSchemaForm.Qualified)]
    public string? Action { get; set; }
}