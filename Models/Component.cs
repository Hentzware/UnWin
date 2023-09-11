using System.Xml.Serialization;
using UnWin.Models;

namespace XmlSerializer.Models;

public class Component
{
    [XmlElement(ElementName = "RegisteredOrganization")]
    public string RegisteredOrganization { get; set; }

    [XmlElement(ElementName = "RegisteredOwner")]
    public string RegisteredOwner { get; set; }

    [XmlElement(ElementName = "SetupUILanguage")]
    public SetupUILanguage SetupUILanguage { get; set; }

    [XmlElement(ElementName = "DiskConfiguration")]
    public DiskConfiguration DiskConfiguration { get; set; }

    [XmlElement(ElementName = "InputLocale")]
    public string InputLocale { get; set; }

    [XmlElement(ElementName = "UserAccounts")]
    public UserAccounts UserAccounts { get; set; }

    [XmlElement(ElementName = "TimeZone")]
    public string TimeZone { get; set; }

    [XmlElement(ElementName = "SystemLocale")]
    public string SystemLocale { get; set; }

    [XmlElement(ElementName = "FirstLogonCommands")]
    public FirstLogonCommands FirstLogonCommands { get; set; }

    [XmlElement(ElementName = "ImageInstall")]
    public ImageInstall ImageInstall { get; set; }

    [XmlElement(ElementName = "InstallFrom")]
    public InstallFrom InstallFrom { get; set; }

    [XmlElement(ElementName = "LogonCommands")]
    public LogonCommands LogonCommands { get; set; }

    [XmlElement(ElementName = "UILanguage")]
    public string UILanguage { get; set; }

    [XmlElement(ElementName = "UserLocale")]
    public string UserLocale { get; set; }

    [XmlElement(ElementName = "OOBE")]
    public OOBE OOBE { get; set; }

    [XmlElement(ElementName = "UserData")]
    public UserData UserData { get; set; }

    [XmlElement(ElementName = "ComputerName")]
    public string ComputerName { get; set; }

    [XmlAttribute(AttributeName = "language")]
    public string Language { get; set; } = "neutral";

    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }

    [XmlAttribute(AttributeName = "processorArchitecture")]
    public string ProcessorArchitecture { get; set; } = "amd64";

    [XmlAttribute(AttributeName = "publicKeyToken")]
    public string PublicKeyToken { get; set; } = "31bf3856ad364e35";

    [XmlAttribute(AttributeName = "versionScope")]
    public string VersionScope { get; set; } = "nonSxS";
}