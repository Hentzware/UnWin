using System.Xml.Serialization;

namespace XmlSerializer.Models;

public class LocalAccounts
{
    [XmlElement(ElementName = "LocalAccount")]
    public LocalAccount LocalAccount { get; set; }
}