using System.Text;
using System.Xml;

namespace UnWin.Services;

public class UnattendService : IUnattendService
{
    private readonly ISettingsService _settingsService;

    public UnattendService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public void SaveAutounattendXmlFile(string filePath)
    {
        var settings = _settingsService.LoadAutounattendSettings();
        var processorArchitecture = "amd64";
        var publicKeyToken = "31bf3856ad364e35";
        var language = "neutral";
        var versionScope = "nonSxS";
        var xmlnsWcm = "http://schemas.microsoft.com/WMIConfig/2002/State";
        var xmlnsXsi = "http://www.w3.org/2001/XMLSchema-instance";
        
        var writerSettings = new XmlWriterSettings()
        {
            Indent = true,
            NewLineChars = "\n\r",
            Encoding = Encoding.UTF8
        };

        using (var writer = XmlWriter.Create(filePath, writerSettings))
        {
            writer.WriteStartDocument();

            writer.WriteStartElement("unattend", "urn:schemas-microsoft-com:unattend");

            //writer.WriteAttributeString("xmlns", null, "urn:schemas-microsoft-com:unattend");


            #region 1. WindowsPE

            writer.WriteStartElement("settings");

            writer.WriteAttributeString("pass", null, "windowsPE");


            #region Microsoft-Windows-International-Core-WinPE

            writer.WriteStartElement("component");

            writer.WriteAttributeString("name", null, "Microsoft-Windows-International-Core-WinPE");
            writer.WriteAttributeString("processorArchitecture", null, processorArchitecture);
            writer.WriteAttributeString("publicKeyToken", null, publicKeyToken);
            writer.WriteAttributeString("language", null, language);
            writer.WriteAttributeString("versionScope", null, versionScope);
            //writer.WriteAttributeString("xmlns:wcm", null, xmlnsWcm);
            //writer.WriteAttributeString("xmlns:xsi", null, xmlnsXsi);

            writer.WriteStartElement("SetupUILanguage");

            writer.WriteElementString("UILanguage", settings.Language);

            writer.WriteEndElement(); // SetupUILanguage

            writer.WriteElementString("InputLocale", settings.Language);

            writer.WriteElementString("SystemLocale", settings.Language);

            writer.WriteElementString("UILanguage", settings.Language);

            writer.WriteElementString("UserLocale", settings.Language);

            writer.WriteEndElement(); // component

            #endregion

            #region Microsoft-Windows-Setup

            writer.WriteStartElement("component");

            writer.WriteAttributeString("name", null, "Microsoft-Windows-Setup");
            writer.WriteAttributeString("processorArchitecture", null, processorArchitecture);
            writer.WriteAttributeString("publicKeyToken", null, publicKeyToken);
            writer.WriteAttributeString("language", null, language);
            writer.WriteAttributeString("versionScope", null, versionScope);
            //writer.WriteAttributeString("xmlns:wcm", null, xmlnsWcm);
            //writer.WriteAttributeString("xmlns:xsi", null, xmlnsXsi);

            writer.WriteStartElement("DiskConfiguration");

            writer.WriteStartElement("Disk");

            writer.WriteAttributeString("wcm", "action", "add");

            writer.WriteStartElement("CreatePartitions");

            writer.WriteStartElement("CreatePartition");

            writer.WriteAttributeString("wcm", "action", "add");

            writer.WriteElementString("Order", "1");
            writer.WriteElementString("Size", settings.EFISize.ToString());
            writer.WriteElementString("Type", "EFI");

            writer.WriteEndElement(); // CreatePartition

            writer.WriteStartElement("CreatePartition");

            writer.WriteAttributeString("wcm", "action", "add");

            writer.WriteElementString("Order", "2");
            writer.WriteElementString("Size", settings.OSSize.ToString());
            writer.WriteElementString("Type", "Primary");

            writer.WriteEndElement(); // CreatePartition

            writer.WriteStartElement("CreatePartition");

            writer.WriteAttributeString("wcm", "action", "add");

            writer.WriteElementString("Order", "3");
            writer.WriteElementString("Size", settings.WinRESize.ToString());
            writer.WriteElementString("Type", "Primary");

            writer.WriteEndElement(); // CreatePartition

            writer.WriteEndElement(); // CreatePartitions

            writer.WriteStartElement("ModifyPartitions");

            writer.WriteStartElement("ModifyPartition");

            writer.WriteAttributeString("wcm", "action", "add");

            writer.WriteElementString("Format", "FAT32");
            writer.WriteElementString("Label", "EFI");
            writer.WriteElementString("Order", "1");
            writer.WriteElementString("PartitionID", "1");

            writer.WriteEndElement(); // ModifyPartition

            writer.WriteStartElement("ModifyPartition");

            writer.WriteAttributeString("wcm", "action", "add");

            writer.WriteElementString("Format", "NTFS");
            writer.WriteElementString("Label", "OS");
            writer.WriteElementString("Order", "2");
            writer.WriteElementString("PartitionID", "2");

            writer.WriteEndElement(); // ModifyPartition

            writer.WriteStartElement("ModifyPartition");

            writer.WriteAttributeString("wcm", "action", "add");

            writer.WriteElementString("Format", "NTFS");
            writer.WriteElementString("Label", "WinRE");
            writer.WriteElementString("Order", "3");
            writer.WriteElementString("PartitionID", "3");

            writer.WriteEndElement(); // ModifyPartition

            writer.WriteEndElement(); // ModifyPartitions

            writer.WriteElementString("DiskID", "0");
            writer.WriteElementString("WillWipeDisk", "true");

            writer.WriteEndElement(); // Disk

            writer.WriteEndElement(); // DiskConfiguration

            writer.WriteStartElement("UserData");

            writer.WriteElementString("AcceptEula", "true");

            writer.WriteEndElement(); // UserData

            writer.WriteStartElement("ImageInstall");

            writer.WriteStartElement("OSImage");

            writer.WriteStartElement("InstallTo");

            writer.WriteElementString("DiskID", "0");
            writer.WriteElementString("PartitionID", "2");

            writer.WriteEndElement(); // InstallTo

            writer.WriteEndElement(); // OSImage

            writer.WriteEndElement(); // ImageInstall

            writer.WriteEndElement(); // Component

            #endregion

            writer.WriteEndElement(); // Settings

            #endregion

            #region 2. Offline Servicing

            writer.WriteStartElement("settings");

            writer.WriteAttributeString("pass", null, "offlineServicing");

            writer.WriteEndElement();

            #endregion

            #region 3. Generalize

            writer.WriteStartElement("settings");

            writer.WriteAttributeString("pass", null, "generalize");

            writer.WriteEndElement();

            #endregion

            #region 4. Specialize

            writer.WriteStartElement("settings");

            writer.WriteAttributeString("pass", null, "specialize");

            writer.WriteStartElement("component");

            writer.WriteAttributeString("name", null, "Microsoft-Windows-Shell-Setup");
            writer.WriteAttributeString("processorArchitecture", null, processorArchitecture);
            writer.WriteAttributeString("publicKeyToken", null, publicKeyToken);
            writer.WriteAttributeString("language", null, language);
            writer.WriteAttributeString("versionScope", null, versionScope);
            //writer.WriteAttributeString("xmlns:wcm", null, xmlnsWcm);
            //writer.WriteAttributeString("xmlns:xsi", null, xmlnsXsi);

            writer.WriteElementString("ComputerName", settings.ComputerName);
            writer.WriteElementString("TimeZone", "W. European Standard Time");

            writer.WriteEndElement(); // component

            writer.WriteEndElement(); // settings

            #endregion

            #region 5. Audit System

            writer.WriteStartElement("settings");

            writer.WriteAttributeString("pass", null, "auditSystem");

            writer.WriteEndElement();

            #endregion

            #region 6. Audit User

            writer.WriteStartElement("settings");

            writer.WriteAttributeString("pass", null, "auditUser");

            writer.WriteEndElement();

            #endregion

            #region 7. OOBE System

            writer.WriteStartElement("settings");

            writer.WriteAttributeString("pass", null, "oobeSystem");

            writer.WriteStartElement("component");

            writer.WriteAttributeString("name", null, "Microsoft-Windows-Shell-Setup");
            writer.WriteAttributeString("processorArchitecture", null, processorArchitecture);
            writer.WriteAttributeString("publicKeyToken", null, publicKeyToken);
            writer.WriteAttributeString("language", null, language);
            writer.WriteAttributeString("versionScope", null, versionScope);
            //writer.WriteAttributeString("xmlns:wcm", null, xmlnsWcm);
            //writer.WriteAttributeString("xmlns:xsi", null, xmlnsXsi);

            writer.WriteStartElement("OOBE");

            writer.WriteElementString("HideEULAPage", "true");
            writer.WriteElementString("HideLocalAccountScreen", "true");
            writer.WriteElementString("HideOEMRegistrationScreen", "true");
            writer.WriteElementString("HideOnlineAccountScreens", "true");
            writer.WriteElementString("HideWirelessSetupInOOBE", "true");
            writer.WriteElementString("ProtectYourPC", "3");

            writer.WriteEndElement(); // OOBE

            writer.WriteStartElement("UserAccounts");

            writer.WriteStartElement("LocalAccounts");

            writer.WriteStartElement("LocalAccount");

            writer.WriteAttributeString("wcm", "action", "add");

            writer.WriteStartElement("Password");

            writer.WriteElementString("Value", "Passw0rd");
            writer.WriteElementString("PlainText", "true");

            writer.WriteEndElement(); // Password

            writer.WriteElementString("DisplayName", settings.UserName);
            writer.WriteElementString("Group", "Administratoren");
            writer.WriteElementString("Name", settings.UserName);

            writer.WriteEndElement(); // LocalAccount

            writer.WriteEndElement(); // LocalAccounts

            writer.WriteEndElement(); // UserAccounts

            writer.WriteElementString("TimeZone", "W. European Standard Time");

            writer.WriteStartElement("FirstLogonCommands");

            foreach (var firstLogonCommand in settings.FirstLogonCommands)
            {
                writer.WriteStartElement("SynchronousCommand");

                writer.WriteAttributeString("wcm", "action", "add");

                writer.WriteElementString("CommandLine", firstLogonCommand.Command);
                writer.WriteElementString("Order", firstLogonCommand.Order.ToString());
                writer.WriteElementString("RequiresUserInput", firstLogonCommand.UserInputRequired.ToString());

                writer.WriteEndElement(); // SynchronousCommand
            }

            writer.WriteEndElement(); // FirstLogonCommands

            writer.WriteStartElement("AutoLogon");

            writer.WriteStartElement("Password");

            writer.WriteElementString("Value", "Passw0rd");
            writer.WriteElementString("PlainText", "true");

            writer.WriteEndElement(); // Password

            writer.WriteEndElement(); // AutoLogon

            writer.WriteEndElement(); // component

            #region Microsoft-Windows-International-Core-WinPE

            writer.WriteStartElement("component");

            writer.WriteAttributeString("name", null, "Microsoft-Windows-International-Core");
            writer.WriteAttributeString("processorArchitecture", null, processorArchitecture);
            writer.WriteAttributeString("publicKeyToken", null, publicKeyToken);
            writer.WriteAttributeString("language", null, language);
            writer.WriteAttributeString("versionScope", null, versionScope);
            //writer.WriteAttributeString("xmlns:wcm", null, xmlnsWcm);
            //writer.WriteAttributeString("xmlns:xsi", null, xmlnsXsi);

            writer.WriteElementString("InputLocale", settings.Language);

            writer.WriteElementString("SystemLocale", settings.Language);

            writer.WriteElementString("UILanguage", settings.Language);

            writer.WriteElementString("UserLocale", settings.Language);

            writer.WriteEndElement(); // component

            #endregion

            writer.WriteEndElement(); // settings

            #endregion

            writer.WriteEndElement(); // unattend

            writer.WriteEndDocument();
        }
    }
}