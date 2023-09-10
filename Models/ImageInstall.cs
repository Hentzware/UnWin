using System.Xml.Serialization;

namespace XmlSerializer.Models;

public class ImageInstall
{
    [XmlElement(ElementName = "OSImage")]
    public OSImage OSImage { get; set; }
}