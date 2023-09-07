using System;
using System.IO;
using System.Text;
using System.Xml;
using UnWin.Models;

namespace UnWin.Services;

public class SettingsService : ISettingsService
{
    private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Settings.xml");

    public void Save(Settings settings)
    {
        var xmlWriterSettings = new XmlWriterSettings
        {
            Indent = true,
            Encoding = Encoding.UTF8,
            NewLineChars = "\r\n"
        };

        using (var xmlWriter = XmlWriter.Create(_filePath, xmlWriterSettings))
        {
            xmlWriter.WriteStartDocument();

            xmlWriter.WriteStartElement("Settings");

            xmlWriter.WriteElementString(nameof(settings.AutoLogonCount), settings.AutoLogonCount.ToString());
            xmlWriter.WriteElementString(nameof(settings.AutoLogonEnabled), settings.AutoLogonEnabled.ToString());
            xmlWriter.WriteElementString(nameof(settings.AutounattendPath), settings.AutounattendPath);
            xmlWriter.WriteElementString(nameof(settings.ComputerName), settings.ComputerName);
            xmlWriter.WriteElementString(nameof(settings.EFISize), settings.EFISize.ToString());
            xmlWriter.WriteElementString(nameof(settings.OSSize), settings.OSSize.ToString());
            xmlWriter.WriteElementString(nameof(settings.WinRESize), settings.WinRESize.ToString());
            xmlWriter.WriteElementString(nameof(settings.ExtractionPath), settings.ExtractionPath);
            xmlWriter.WriteElementString(nameof(settings.OscdimgPath), settings.OscdimgPath);
            xmlWriter.WriteElementString(nameof(settings.SourceIsoPath), settings.SourceIsoPath);
            xmlWriter.WriteElementString(nameof(settings.TargetIsoPath), settings.TargetIsoPath);
            xmlWriter.WriteElementString(nameof(settings.UserName), settings.UserName);
            xmlWriter.WriteElementString(nameof(settings.VersionIndex), settings.VersionIndex.ToString());

            xmlWriter.WriteStartElement("FirstLogonCommands");

            foreach (var command in settings.FirstLogonCommands)
            {
                xmlWriter.WriteStartElement("Command");

                xmlWriter.WriteElementString("Order", command.Order.ToString());
                xmlWriter.WriteElementString("Command", command.Command);
                xmlWriter.WriteElementString("UserInputRequired", command.UserInputRequired.ToString());

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();

            foreach (var command in settings.LogonCommands)
            {
                xmlWriter.WriteStartElement("Command");

                xmlWriter.WriteElementString("Order", command.Order.ToString());
                xmlWriter.WriteElementString("Command", command.Command);
                xmlWriter.WriteElementString("UserInputRequired", command.UserInputRequired.ToString());

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndDocument();
        }
    }

    public Settings Load()
    {
        var settings = new Settings();

        using (var xmlReader = XmlReader.Create(_filePath))
        {
            while (xmlReader.Read())
            {
                switch (xmlReader.Name)
                {
                    case nameof(settings.AutoLogonEnabled):
                        settings.AutoLogonEnabled = Convert.ToBoolean(xmlReader.ReadElementContentAsString());
                        break;

                    case nameof(settings.AutoLogonCount):
                        settings.AutoLogonCount = Convert.ToInt32(xmlReader.ReadElementContentAsString());
                        break;

                    case nameof(settings.AutounattendPath):
                        settings.AutounattendPath = xmlReader.ReadElementContentAsString();
                        break;

                    case nameof(settings.OSSize):
                        settings.OSSize = Convert.ToInt32(xmlReader.ReadElementContentAsString());
                        break;

                    case nameof(settings.EFISize):
                        settings.EFISize = Convert.ToInt32(xmlReader.ReadElementContentAsString());
                        break;

                    case nameof(settings.WinRESize):
                        settings.WinRESize = Convert.ToInt32(xmlReader.ReadElementContentAsString());
                        break;

                    case nameof(settings.VersionIndex):
                        settings.VersionIndex = Convert.ToInt32(xmlReader.ReadElementContentAsString());
                        break;

                    case nameof(settings.OscdimgPath):
                        settings.OscdimgPath = xmlReader.ReadElementContentAsString();
                        break;

                    case nameof(settings.SourceIsoPath):
                        settings.SourceIsoPath = xmlReader.ReadElementContentAsString();
                        break;

                    case nameof(settings.ExtractionPath):
                        settings.ExtractionPath = xmlReader.ReadElementContentAsString();
                        break;

                    case nameof(settings.TargetIsoPath):
                        settings.TargetIsoPath = xmlReader.ReadElementContentAsString();
                        break;

                    case nameof(settings.ComputerName):
                        settings.ComputerName = xmlReader.ReadElementContentAsString();
                        break;

                    case nameof(settings.UserName):
                        settings.UserName = xmlReader.ReadElementContentAsString();
                        break;
                }
            }
        }

        return settings;
    }
}