using System.Xml.Serialization;

namespace XmlSerializer.Models;

public class SetupUILanguage
{
    [XmlElement(ElementName = "UILanguage")]
    public string UILanguage { get; set; }
}