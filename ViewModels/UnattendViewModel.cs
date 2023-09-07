using System;
using System.Collections.Generic;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using UnWin.Models;
using UnWin.Properties;
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
        AutoLogonCount = Properties.Settings.Default.AutoLogonCount;
        AutoLogonEnabled = Properties.Settings.Default.AutoLogonEnabled;
        EFISize = Properties.Settings.Default.EFISize;
        OSSize = Properties.Settings.Default.OSSize;
        WinRESize = Properties.Settings.Default.WinRESize;
        ComputerName = Properties.Settings.Default.ComputerName;
        UserName = Properties.Settings.Default.UserName;
        Language = Properties.Settings.Default.Language;
        VersionIndex = Properties.Settings.Default.VersionIndex;
        FirstLogonCommands = new List<LogonCommand>();
        LogonCommands = new List<LogonCommand>();

        var currentSettings = new Models.Settings();
        currentSettings.AutoLogonCount = AutoLogonCount;
        currentSettings.AutoLogonEnabled = AutoLogonEnabled;

        settingsService.Save(currentSettings);

        if (!string.IsNullOrEmpty(Properties.Settings.Default.FirstLogonCommands))
        {
            var firstLogonCommand = Properties.Settings.Default.FirstLogonCommands.Split(';');

            foreach (var s in firstLogonCommand)
            {
                var command = s.Split(',');

                foreach (var c in command)
                {
                    var newCommand = new LogonCommand
                    {
                        Command = s[0].ToString(),
                        Order = Convert.ToInt32(s[1]),
                        UserInputRequired = Convert.ToBoolean(s[2])
                    };

                    FirstLogonCommands.Add(newCommand);
                }
            }

            FirstLogonCommands = new List<LogonCommand>(FirstLogonCommands);
        }

        if (!string.IsNullOrEmpty(Properties.Settings.Default.LogonCommands))
        {
            var logonCommands = Properties.Settings.Default.LogonCommands.Split(';');

            foreach (var s in logonCommands)
            {
                var command = s.Split(',');

                foreach (var c in command)
                {
                    var newCommand = new LogonCommand
                    {
                        Command = s[0].ToString(),
                        Order = Convert.ToInt32(s[1]),
                        UserInputRequired = Convert.ToBoolean(s[2])
                    };

                    LogonCommands.Add(newCommand);
                }
            }

            LogonCommands = new List<LogonCommand>(LogonCommands);
        }
    }

    public bool AutoLogonEnabled
    {
        get => _autoLogonEnabled;
        set
        {
            SetProperty(ref _autoLogonEnabled, value);
            Properties.Settings.Default.AutoLogonEnabled = value;
            Properties.Settings.Default.Save();
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
            Properties.Settings.Default.AutoLogonCount = value;
            Properties.Settings.Default.Save();
            RaisePropertyChanged();
        }
    }

    public int EFISize
    {
        get => _efiSize;
        set
        {
            SetProperty(ref _efiSize, value);
            Properties.Settings.Default.EFISize = value;
            Properties.Settings.Default.Save();
            RaisePropertyChanged();
        }
    }

    public int OSSize
    {
        get => _osSize;
        set
        {
            SetProperty(ref _osSize, value);
            Properties.Settings.Default.OSSize = value;
            Properties.Settings.Default.Save();
            RaisePropertyChanged();
        }
    }

    public int VersionIndex
    {
        get => _versionIndex;
        set
        {
            SetProperty(ref _versionIndex, value);
            Properties.Settings.Default.VersionIndex = value;
            Properties.Settings.Default.Save();
            RaisePropertyChanged();
        }
    }

    public int WinRESize
    {
        get => _winRESize;
        set
        {
            SetProperty(ref _winRESize, value);
            Properties.Settings.Default.WinRESize = value;
            Properties.Settings.Default.Save();
            RaisePropertyChanged();
        }
    }

    public List<LogonCommand> FirstLogonCommands
    {
        get => _firstLogonCommands;
        set
        {
            SetProperty(ref _firstLogonCommands, value);

            var commands = string.Empty;

            foreach (var flc in _firstLogonCommands)
            {
                commands += flc.Command + ",";
                commands += flc.Order + ",";
                commands += flc.UserInputRequired + ";";
            }

            Properties.Settings.Default.FirstLogonCommands = commands;
            Properties.Settings.Default.Save();
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
            Properties.Settings.Default.ComputerName = value;
            Properties.Settings.Default.Save();
            RaisePropertyChanged();
        }
    }

    public string Language
    {
        get => _language;
        set
        {
            SetProperty(ref _language, value);
            Properties.Settings.Default.Language = value;
            Properties.Settings.Default.Save();
            RaisePropertyChanged();
        }
    }

    public string UserName
    {
        get => _userName;
        set
        {
            SetProperty(ref _userName, value);
            Properties.Settings.Default.UserName = value;
            Properties.Settings.Default.Save();
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