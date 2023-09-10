using System.Xml.Serialization;

namespace XmlSerializer.Models;

public class Password
{
    [XmlElement(ElementName = "Value")]
    public string Value { get; set; }

    [XmlElement(ElementName = "PlainText")]
    public bool PlainText { get; set; }
}