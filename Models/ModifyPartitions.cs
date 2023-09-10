using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlSerializer.Models;

public class ModifyPartitions
{
    [XmlElement(ElementName = "ModifyPartition")]
    public List<ModifyPartition> ModifyPartition { get; set; }
}