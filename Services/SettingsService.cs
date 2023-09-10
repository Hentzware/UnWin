using System.IO;
using UnWin.Models;

namespace UnWin.Services;

public class SettingsService : ISettingsService
{
    private readonly string _autounattendSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "AutounattendSettings.xml");
    private readonly string _imageSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "ImageSettings.xml");

    public AutounattendSettings LoadAutounattendSettings()
    {
        var settings = new AutounattendSettings();
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(AutounattendSettings));

        if (!File.Exists(_autounattendSettingsPath))
        {
            return settings;
        }

        using (var reader = new StreamReader(_autounattendSettingsPath))
        {
            settings = (AutounattendSettings)serializer.Deserialize(reader);
            return settings;
        }
    }

    public ImageSettings LoadImageSettings()
    {
        var settings = new ImageSettings();
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ImageSettings));

        if (!File.Exists(_imageSettingsPath))
        {
            return settings;
        }

        using (var reader = new StreamReader(_imageSettingsPath))
        {
            settings = (ImageSettings)serializer.Deserialize(reader);
            return settings;
        }
    }

    public void SaveAutounattendSettings(AutounattendSettings settings)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(AutounattendSettings));

        using (var stream = new StreamWriter(_autounattendSettingsPath))
        {
            serializer.Serialize(stream, settings);
        }
    }

    public void SaveImageSettings(ImageSettings settings)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ImageSettings));

        using (var stream = new StreamWriter(_imageSettingsPath))
        {
            serializer.Serialize(stream, settings);
        }
    }
}