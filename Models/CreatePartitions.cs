using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlSerializer.Models;

public class CreatePartitions
{
    [XmlElement(ElementName = "CreatePartition")]
    public List<CreatePartition> CreatePartition { get; set; }
}