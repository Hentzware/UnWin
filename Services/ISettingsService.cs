using UnWin.Models;

namespace UnWin.Services;

public interface ISettingsService
{
    Settings Load();
    void Save(Settings settings);
}