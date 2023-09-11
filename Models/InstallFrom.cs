using System.Xml.Serialization;

namespace UnWin.Models;

public class InstallFrom
{
    [XmlElement(ElementName = "MetaData")]
    public MetaData MetaData { get; set; }
}