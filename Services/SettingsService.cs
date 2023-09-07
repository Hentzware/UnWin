using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnWin.Models;

namespace UnWin.Services;

public class SettingsService : ISettingsService
{
    private readonly string _autounattendSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "AutounattendSettings.xml");
    private readonly string _imageSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "ImageSettings.xml");

    public AutounattendSettings LoadAutounattendSettings()
    {
        var settings = new AutounattendSettings();
        settings.FirstLogonCommands = new List<LogonCommand>();
        settings.LogonCommands = new List<LogonCommand>();

        if (!File.Exists(_autounattendSettingsPath))
        {
            return settings;
        }

        using (var xmlReader = XmlReader.Create(_autounattendSettingsPath))
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

                    case nameof(settings.ComputerName):
                        settings.ComputerName = xmlReader.ReadElementContentAsString();
                        break;

                    case nameof(settings.UserName):
                        settings.UserName = xmlReader.ReadElementContentAsString();
                        break;

                    case nameof(settings.Language):
                        settings.Language = xmlReader.ReadElementContentAsString();
                        break;

                    case "FirstLogonCommand":
                        var firstLogonCommand = new LogonCommand()
                        {
                            Order = Convert.ToInt32(xmlReader.GetAttribute("Order")),
                            Command = xmlReader.GetAttribute("Command"),
                            UserInputRequired = Convert.ToBoolean(xmlReader.GetAttribute("UserInputRequired"))
                        };
                        settings.FirstLogonCommands.Add(firstLogonCommand);
                        break;

                    case "LogonCommand":
                        var logonCommand = new LogonCommand()
                        {
                            Order = Convert.ToInt32(xmlReader.GetAttribute("Order")),
                            Command = xmlReader.GetAttribute("Command"),
                            UserInputRequired = Convert.ToBoolean(xmlReader.GetAttribute("UserInputRequired"))
                        };
                        settings.LogonCommands.Add(logonCommand);
                        break;
                }
            }
        }

        return settings;
    }

    public ImageSettings LoadImageSettings()
    {
        var settings = new ImageSettings();

        if (!File.Exists(_imageSettingsPath))
        {
            return settings;
        }

        using (var xmlReader = XmlReader.Create(_imageSettingsPath))
        {
            while (xmlReader.Read())
            {
                switch (xmlReader.Name)
                {
                    case nameof(settings.AutounattendPath):
                        settings.AutounattendPath = xmlReader.ReadElementContentAsString();
                        break;

                    case nameof(settings.AutounattendMode):
                        settings.AutounattendMode = Convert.ToInt32(xmlReader.ReadElementContentAsString());
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
                }
            }
        }

        return settings;
    }

    public void SaveAutounattendSettings(AutounattendSettings settings)
    {
        if (settings.FirstLogonCommands == null)
        {
            settings.FirstLogonCommands = new List<LogonCommand>();
        }

        if (settings.LogonCommands == null)
        {
            settings.LogonCommands = new List<LogonCommand>();
        }

        var xmlWriterSettings = new XmlWriterSettings
        {
            Indent = true,
            Encoding = Encoding.UTF8,
            NewLineChars = "\r\n"
        };

        using (var xmlWriter = XmlWriter.Create(_autounattendSettingsPath, xmlWriterSettings))
        {
            xmlWriter.WriteStartDocument();

            xmlWriter.WriteStartElement("Settings");

            xmlWriter.WriteElementString(nameof(settings.AutoLogonCount), settings.AutoLogonCount.ToString());
            xmlWriter.WriteElementString(nameof(settings.AutoLogonEnabled), settings.AutoLogonEnabled.ToString());
            xmlWriter.WriteElementString(nameof(settings.ComputerName), settings.ComputerName);
            xmlWriter.WriteElementString(nameof(settings.EFISize), settings.EFISize.ToString());
            xmlWriter.WriteElementString(nameof(settings.OSSize), settings.OSSize.ToString());
            xmlWriter.WriteElementString(nameof(settings.WinRESize), settings.WinRESize.ToString());
            xmlWriter.WriteElementString(nameof(settings.UserName), settings.UserName);
            xmlWriter.WriteElementString(nameof(settings.VersionIndex), settings.VersionIndex.ToString());
            xmlWriter.WriteElementString(nameof(settings.Language), settings.Language);

            foreach (var command in settings.FirstLogonCommands)
            {
                xmlWriter.WriteStartElement("FirstLogonCommand");

                xmlWriter.WriteAttributeString("Order", command.Order.ToString());
                xmlWriter.WriteAttributeString("Command", command.Command);
                xmlWriter.WriteAttributeString("UserInputRequired", command.UserInputRequired.ToString());

                xmlWriter.WriteEndElement();
            }

            foreach (var command in settings.LogonCommands)
            {
                xmlWriter.WriteStartElement("LogonCommand");

                xmlWriter.WriteAttributeString("Order", command.Order.ToString());
                xmlWriter.WriteAttributeString("Command", command.Command);
                xmlWriter.WriteAttributeString("UserInputRequired", command.UserInputRequired.ToString());

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndDocument();
        }
    }

    public void SaveImageSettings(ImageSettings settings)
    {
        var xmlWriterSettings = new XmlWriterSettings
        {
            Indent = true,
            Encoding = Encoding.UTF8,
            NewLineChars = "\r\n"
        };

        using (var xmlWriter = XmlWriter.Create(_imageSettingsPath, xmlWriterSettings))
        {
            xmlWriter.WriteStartDocument();

            xmlWriter.WriteStartElement("Settings");

            xmlWriter.WriteElementString(nameof(settings.AutounattendPath), settings.AutounattendPath);
            xmlWriter.WriteElementString(nameof(settings.AutounattendMode), settings.AutounattendMode.ToString());
            xmlWriter.WriteElementString(nameof(settings.ExtractionPath), settings.ExtractionPath);
            xmlWriter.WriteElementString(nameof(settings.OscdimgPath), settings.OscdimgPath);
            xmlWriter.WriteElementString(nameof(settings.SourceIsoPath), settings.SourceIsoPath);
            xmlWriter.WriteElementString(nameof(settings.TargetIsoPath), settings.TargetIsoPath);

            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndDocument();
        }
    }
}