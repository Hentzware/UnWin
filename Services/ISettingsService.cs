using UnWin.Models;

namespace UnWin.Services;

public interface ISettingsService
{
    AutounattendSettings LoadAutounattendSettings();

    ImageSettings LoadImageSettings();

    void SaveAutounattendSettings(AutounattendSettings settings);

    void SaveImageSettings(ImageSettings settings);
}