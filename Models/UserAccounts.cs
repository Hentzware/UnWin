using System.Xml.Serialization;

namespace XmlSerializer.Models;

public class UserAccounts
{
    [XmlElement(ElementName = "LocalAccounts")]
    public LocalAccounts LocalAccounts { get; set; }

    [XmlElement(ElementName = "AdministratorPassword")]
    public Password AdministratorPassword { get; set; }
}