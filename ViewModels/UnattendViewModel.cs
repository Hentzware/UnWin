using System.Collections.Generic;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using UnWin.Models;
using UnWin.Services;
using UnWin.Views;

namespace UnWin.ViewModels;

public class UnattendViewModel : BindableBase
{
    private readonly IDialogService _dialogService;
    private readonly IRegionManager _regionManager;
    private readonly ISettingsService _settingsService;
    private bool _autoLogonEnabled;
    private bool _createLocalAccount;
    private bool _versionIndexEnabled;
    private DelegateCommand _navigateBackCommand;
    private DelegateCommand<string> _deleteLogonCommand;
    private DelegateCommand<string> _openLogonCommandDialog;
    private int _autoLogonCount;
    private int _efiSize;
    private int _firstLogonCommandSelectedIndex;
    private int _logonCommandSelectedIndex;
    private int _osSize;
    private int _versionIndex;
    private int _winRESize;
    private List<LogonCommand> _firstLogonCommands;
    private List<LogonCommand> _logonCommands;
    private string _administratorPassword;
    private string _computerName;
    private string _displayName;
    private string _language;
    private string _password;
    private string _userName;

    public UnattendViewModel(IRegionManager regionManager, IDialogService dialogService, ISettingsService settingsService)
    {
        _regionManager = regionManager;
        _dialogService = dialogService;
        _settingsService = settingsService;

        LoadSettings();
    }

    public DelegateCommand<string> DeleteLogonCommand =>
        _deleteLogonCommand ?? new DelegateCommand<string>(ExecuteDeleteLogonCommand);

    public DelegateCommand NavigateBackCommand =>
        _navigateBackCommand ?? new DelegateCommand(ExecuteNavigateBackCommand);

    public DelegateCommand<string> OpenLogonCommandDialog =>
        _openLogonCommandDialog ?? new DelegateCommand<string>(ExecuteOpenLogonCommandDialog);

    public bool AdministratorPasswordEnabled => !_createLocalAccount;

    public bool AutoLogonEnabled
    {
        get => _autoLogonEnabled;
        set
        {
            SetProperty(ref _autoLogonEnabled, value);
            RaisePropertyChanged();
            SaveSettings();
        }
    }

    public bool CreateLocalAccount
    {
        get => _createLocalAccount;
        set
        {
            SetProperty(ref _createLocalAccount, value);
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(AdministratorPasswordEnabled));
            SaveSettings();
        }
    }

    public bool VersionIndexEnabled
    {
        get => _versionIndexEnabled;
        set
        {
            SetProperty(ref _versionIndexEnabled, value);
            RaisePropertyChanged();
            SaveSettings();
        }
    }

    public int AutoLogonCount
    {
        get => _autoLogonCount;
        set
        {
            SetProperty(ref _autoLogonCount, value);
            RaisePropertyChanged();
            SaveSettings();
        }
    }

    public int EFISize
    {
        get => _efiSize;
        set
        {
            SetProperty(ref _efiSize, value);
            RaisePropertyChanged();
            SaveSettings();
        }
    }

    public int FirstLogonCommandSelectedIndex
    {
        get => _firstLogonCommandSelectedIndex;
        set
        {
            SetProperty(ref _firstLogonCommandSelectedIndex, value);
            RaisePropertyChanged();
        }
    }

    public int LogonCommandSelectedIndex
    {
        get => _logonCommandSelectedIndex;
        set
        {
            SetProperty(ref _logonCommandSelectedIndex, value);
            RaisePropertyChanged();
        }
    }

    public int OSSize
    {
        get => _osSize;
        set
        {
            SetProperty(ref _osSize, value);
            RaisePropertyChanged();
            SaveSettings();
        }
    }

    public int VersionIndex
    {
        get => _versionIndex;
        set
        {
            SetProperty(ref _versionIndex, value);
            RaisePropertyChanged();
            SaveSettings();
        }
    }

    public int WinRESize
    {
        get => _winRESize;
        set
        {
            SetProperty(ref _winRESize, value);
            RaisePropertyChanged();
            SaveSettings();
        }
    }

    public List<LogonCommand> FirstLogonCommands
    {
        get => _firstLogonCommands;
        set
        {
            SetProperty(ref _firstLogonCommands, value);
            RaisePropertyChanged();
            SaveSettings();
        }
    }

    public List<LogonCommand> LogonCommands
    {
        get => _logonCommands;
        set
        {
            SetProperty(ref _logonCommands, value);
            RaisePropertyChanged();
            SaveSettings();
        }
    }

    public string AdministratorPassword
    {
        get => _administratorPassword;
        set
        {
            SetProperty(ref _administratorPassword, value);
            RaisePropertyChanged();
            SaveSettings();
        }
    }

    public string ComputerName
    {
        get => _computerName;
        set
        {
            SetProperty(ref _computerName, value);
            RaisePropertyChanged();
            SaveSettings();
        }
    }

    public string DisplayName
    {
        get => _displayName;
        set
        {
            SetProperty(ref _displayName, value);
            RaisePropertyChanged();
            SaveSettings();
        }
    }

    public string Language
    {
        get => _language;
        set
        {
            SetProperty(ref _language, value);
            RaisePropertyChanged();
            SaveSettings();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            SetProperty(ref _password, value);
            RaisePropertyChanged();
            SaveSettings();
        }
    }

    public string UserName
    {
        get => _userName;
        set
        {
            SetProperty(ref _userName, value);
            RaisePropertyChanged();
            SaveSettings();
        }
    }

    private void ExecuteDeleteLogonCommand(string ctx)
    {
        switch (ctx)
        {
            case "FirstLogonCommand":
                if (_firstLogonCommandSelectedIndex > -1)
                {
                    FirstLogonCommands.RemoveAt(_firstLogonCommandSelectedIndex);
                    FirstLogonCommands = new List<LogonCommand>(FirstLogonCommands);
                }

                break;

            case "LogonCommand":
                if (_logonCommandSelectedIndex > -1)
                {
                    LogonCommands.RemoveAt(_logonCommandSelectedIndex);
                    LogonCommands = new List<LogonCommand>(LogonCommands);
                }

                break;
        }
    }

    private void ExecuteNavigateBackCommand()
    {
        _regionManager.RequestNavigate("ContentRegion", nameof(ImageView));
    }

    private void ExecuteOpenLogonCommandDialog(string ctx)
    {
        _dialogService.ShowDialog(nameof(LogonCommandDialogView), null, result =>
        {
            var cmd = new LogonCommand
            {
                Command = result.Parameters.GetValue<string>("Command"),
                Order = result.Parameters.GetValue<int>("Order"),
                RequiresUserInput = result.Parameters.GetValue<bool>("RequiresUserInput")
            };

            if (result.Result == ButtonResult.Cancel || result.Result == ButtonResult.None)
            {
                return;
            }

            if (ctx == "FirstLogon")
            {
                FirstLogonCommands.Add(cmd);
                FirstLogonCommands = new List<LogonCommand>(FirstLogonCommands);
                return;
            }

            LogonCommands.Add(cmd);
            LogonCommands = new List<LogonCommand>(LogonCommands);
        });
    }

    private void LoadSettings()
    {
        var autounattendSettings = _settingsService.LoadAutounattendSettings();

        AutoLogonCount = autounattendSettings.AutoLogonCount;
        AutoLogonEnabled = autounattendSettings.AutoLogonEnabled;
        ComputerName = autounattendSettings.ComputerName;
        EFISize = autounattendSettings.EFISize;
        FirstLogonCommands = autounattendSettings.FirstLogonCommands;
        Language = autounattendSettings.Language;
        LogonCommands = autounattendSettings.LogonCommands;
        OSSize = autounattendSettings.OSSize;
        UserName = autounattendSettings.Name;
        VersionIndex = autounattendSettings.VersionIndex;
        WinRESize = autounattendSettings.WinRESize;
        VersionIndexEnabled = autounattendSettings.VersionIndexEnabled;
        CreateLocalAccount = autounattendSettings.CreateLocalAccount;
        AdministratorPassword = autounattendSettings.AdministratorPassword;
        Password = autounattendSettings.Password;
        DisplayName = autounattendSettings.DisplayName;
        
    }

    private void SaveSettings()
    {
        var autounattendSettings = new AutounattendSettings
        {
            AutoLogonCount = _autoLogonCount,
            AutoLogonEnabled = _autoLogonEnabled,
            ComputerName = _computerName,
            EFISize = _efiSize,
            FirstLogonCommands = _firstLogonCommands,
            Language = _language,
            LogonCommands = _logonCommands,
            OSSize = _osSize,
            Name = _userName,
            VersionIndex = _versionIndex,
            WinRESize = _winRESize,
            CreateLocalAccount = _createLocalAccount,
            VersionIndexEnabled = _versionIndexEnabled,
            AdministratorPassword = _administratorPassword,
            Password = _password,
            DisplayName = _displayName
        };

        _settingsService.SaveAutounattendSettings(autounattendSettings);
    }
}