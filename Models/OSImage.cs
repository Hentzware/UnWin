using System.Xml.Serialization;

namespace XmlSerializer.Models;

public class OSImage
{
    [XmlElement(ElementName = "InstallTo")]
    public InstallTo InstallTo { get; set; }

    [XmlElement(ElementName = "WillShowUI")]
    public string WillShowUI { get; set; }
}