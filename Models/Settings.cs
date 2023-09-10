using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlSerializer.Models;

public class Settings
{
    [XmlElement(ElementName = "component")]
    public List<Component> Components { get; set; }

    [XmlAttribute(AttributeName = "pass")]
    public string Pass { get; set; }
}