using System.Xml.Serialization;

namespace XmlSerializer.Models;

public class OOBE
{
    [XmlElement(ElementName = "HideEULAPage")]
    public bool HideEULAPage { get; set; }

    [XmlElement(ElementName = "HideLocalAccountScreen")]
    public bool HideLocalAccountScreen { get; set; }

    [XmlElement(ElementName = "HideOEMRegistrationScreen")]
    public bool HideOEMRegistrationScreen { get; set; }

    [XmlElement(ElementName = "HideOnlineAccountScreens")]
    public bool HideOnlineAccountScreens { get; set; }

    [XmlElement(ElementName = "HideWirelessSetupInOOBE")]
    public bool HideWirelessSetupInOOBE { get; set; }

    [XmlElement(ElementName = "ProtectYourPC")]
    public int ProtectYourPC { get; set; }
}