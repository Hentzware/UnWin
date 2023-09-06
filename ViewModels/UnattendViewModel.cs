using System;
using System.Collections.Generic;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using UnWin.Models;
using UnWin.Properties;
using UnWin.Views;

namespace UnWin.ViewModels;

public class UnattendViewModel : BindableBase
{
    private readonly IRegionManager _regionManager;
    private readonly IDialogService _dialogService;
    private bool _autoLogonEnabled;
    private int _autoLogonCount;
    private int _efiSize;
    private int _osSize;
    private int _winRESize;
    private List<LogonCommand> _firstLogonCommands;
    private List<LogonCommand> _logonCommands;
    private string _computerName;
    private string _language;
    private string _userName;
    private int _versionIndex;
    private DelegateCommand _openLogonCommandDialog;

    public DelegateCommand OpenLogonCommandDialog =>
        _openLogonCommandDialog ?? new DelegateCommand(ExecuteOpenLogonCommandDialog);

    private void ExecuteOpenLogonCommandDialog()
    {
        _dialogService.ShowDialog(nameof(LogonCommandDialogView));
    }

    public int VersionIndex
    {
        get => _versionIndex;
        set
        {
            SetProperty(ref _versionIndex, value);
            Settings.Default.VersionIndex = value;
            Settings.Default.Save();
            RaisePropertyChanged();
        }
    }

    public UnattendViewModel(IRegionManager regionManager, IDialogService dialogService)
    {
        _regionManager = regionManager;
        _dialogService = dialogService;
        AutoLogonCount = Settings.Default.AutoLogonCount;
        AutoLogonEnabled = Settings.Default.AutoLogonEnabled;
        EFISize = Settings.Default.EFISize;
        OSSize = Settings.Default.OSSize;
        WinRESize = Settings.Default.WinRESize;
        ComputerName = Settings.Default.ComputerName;
        UserName = Settings.Default.UserName;
        Language = Settings.Default.Language;
        VersionIndex = Settings.Default.VersionIndex;
        FirstLogonCommands = new List<LogonCommand>();
        LogonCommands = new List<LogonCommand>();

        if (!string.IsNullOrEmpty(Settings.Default.FirstLogonCommands))
        {
            var firstLogonCommand = Settings.Default.FirstLogonCommands.Split(';');

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
        }

        if (!string.IsNullOrEmpty(Settings.Default.LogonCommands))
        {
            var logonCommands = Settings.Default.LogonCommands.Split(';');

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
        }
    }

    public bool AutoLogonEnabled
    {
        get => _autoLogonEnabled;
        set
        {
            SetProperty(ref _autoLogonEnabled, value);
            Settings.Default.AutoLogonEnabled = value;
            Settings.Default.Save();
            RaisePropertyChanged();
        }
    }

    public int AutoLogonCount
    {
        get => _autoLogonCount;
        set
        {
            SetProperty(ref _autoLogonCount, value);
            Settings.Default.AutoLogonCount = value;
            Settings.Default.Save();
            RaisePropertyChanged();
        }
    }

    public int EFISize
    {
        get => _efiSize;
        set
        {
            SetProperty(ref _efiSize, value);
            Settings.Default.EFISize = value;
            Settings.Default.Save();
            RaisePropertyChanged();
        }
    }

    public int OSSize
    {
        get => _osSize;
        set
        {
            SetProperty(ref _osSize, value);
            Settings.Default.OSSize = value;
            Settings.Default.Save();
            RaisePropertyChanged();
        }
    }

    public int WinRESize
    {
        get => _winRESize;
        set
        {
            SetProperty(ref _winRESize, value);
            Settings.Default.WinRESize = value;
            Settings.Default.Save();
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

            Settings.Default.FirstLogonCommands = commands;
            Settings.Default.Save();
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
            Settings.Default.ComputerName = value;
            Settings.Default.Save();
            RaisePropertyChanged();
        }
    }

    public string Language
    {
        get => _language;
        set
        {
            SetProperty(ref _language, value);
            Settings.Default.Language = value;
            Settings.Default.Save();
            RaisePropertyChanged();
        }
    }

    public string UserName
    {
        get => _userName;
        set
        {
            SetProperty(ref _userName, value);
            Settings.Default.UserName = value;
            Settings.Default.Save();
            RaisePropertyChanged();
        }
    }
}