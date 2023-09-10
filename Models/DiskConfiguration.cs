using System.Xml.Serialization;

namespace XmlSerializer.Models;

public class DiskConfiguration
{
    [XmlElement(ElementName = "Disk")]
    public Disk Disk { get; set; }
}