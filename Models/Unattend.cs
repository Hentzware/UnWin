using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlSerializer.Models;

[XmlRoot(ElementName = "unattend", Namespace = "urn:schemas-microsoft-com:unattend")]
public class Unattend
{
    [XmlElement(ElementName = "settings")]
    public List<Settings> Settings { get; set; }
}