using System.Xml.Serialization;

namespace XmlSerializer.Models;

public class InstallTo
{
    [XmlElement(ElementName = "DiskID")]
    public int DiskID { get; set; }

    [XmlElement(ElementName = "PartitionID")]
    public int PartitionID { get; set; }
}