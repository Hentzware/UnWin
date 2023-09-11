using System.Xml.Serialization;
using UnWin.Models;

namespace XmlSerializer.Models;

public class OSImage
{
    [XmlElement(ElementName = "InstallTo")]
    public InstallTo InstallTo { get; set; }

    [XmlElement(ElementName = "InstallFrom")]
    public InstallFrom InstallFrom { get; set; }


    [XmlElement(ElementName = "WillShowUI")]
    public string WillShowUI { get; set; }
}