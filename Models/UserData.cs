using System.Xml.Serialization;

namespace XmlSerializer.Models;

public class UserData
{
    [XmlElement(ElementName = "AcceptEula")]
    public bool AcceptEula { get; set; }
}