using System;
using System.Collections.Generic;
using Prism.Commands;
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
    private bool _autoLogonEnabled;
    private DelegateCommand _navigateBackCommand;
    private DelegateCommand<string> _openLogonCommandDialog;
    private int _autoLogonCount;
    private int _efiSize;
    private int _osSize;
    private int _versionIndex;
    private int _winRESize;
    private List<LogonCommand> _firstLogonCommands;
    private List<LogonCommand> _logonCommands;
    private string _computerName;
    private string _language;
    private string _userName;

    public UnattendViewModel(IRegionManager regionManager, IDialogService dialogService, ISettingsService settingsService)
    {
        _regionManager = regionManager;
        _dialogService = dialogService;
        FirstLogonCommands = new List<LogonCommand>();
        LogonCommands = new List<LogonCommand>();

        var currentSettings = new Models.Settings();
        currentSettings.AutoLogonCount = AutoLogonCount;
        currentSettings.AutoLogonEnabled = AutoLogonEnabled;

        settingsService.Save(currentSettings);
    }

    public bool AutoLogonEnabled
    {
        get => _autoLogonEnabled;
        set
        {
            SetProperty(ref _autoLogonEnabled, value);
            RaisePropertyChanged();
        }
    }

    public DelegateCommand NavigateBackCommand =>
        _navigateBackCommand ?? new DelegateCommand(ExecuteNavigateBackCommand);

    public DelegateCommand<string> OpenLogonCommandDialog =>
        _openLogonCommandDialog ?? new DelegateCommand<string>(ExecuteOpenLogonCommandDialog);

    public int AutoLogonCount
    {
        get => _autoLogonCount;
        set
        {
            SetProperty(ref _autoLogonCount, value);
            RaisePropertyChanged();
        }
    }

    public int EFISize
    {
        get => _efiSize;
        set
        {
            SetProperty(ref _efiSize, value);
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
        }
    }

    public int VersionIndex
    {
        get => _versionIndex;
        set
        {
            SetProperty(ref _versionIndex, value);
            RaisePropertyChanged();
        }
    }

    public int WinRESize
    {
        get => _winRESize;
        set
        {
            SetProperty(ref _winRESize, value);
            RaisePropertyChanged();
        }
    }

    public List<LogonCommand> FirstLogonCommands
    {
        get => _firstLogonCommands;
        set
        {
            SetProperty(ref _firstLogonCommands, value);
            RaisePropertyChanged();
        }
    }

    public List<LogonCommand> LogonCommands
    {
        get => _logonCommands;
        set
        {
            SetProperty(ref _logonCommands, value);
            RaisePropertyChanged();
        }
    }

    public string ComputerName
    {
        get => _computerName;
        set
        {
            SetProperty(ref _computerName, value);
            RaisePropertyChanged();
        }
    }

    public string Language
    {
        get => _language;
        set
        {
            SetProperty(ref _language, value);
            RaisePropertyChanged();
        }
    }

    public string UserName
    {
        get => _userName;
        set
        {
            SetProperty(ref _userName, value);
            RaisePropertyChanged();
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
                UserInputRequired = result.Parameters.GetValue<bool>("UserInputRequired")
            };

            if (result.Result == ButtonResult.Cancel) return;

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
}