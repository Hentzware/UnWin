using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace UnWin.ViewModels;

public class LogonCommandDialogViewModel : BindableBase, IDialogAware
{
    private bool _userInputRequired;
    private DelegateCommand<string> _closeCommand;
    private int _order;
    private string _command;

    public DelegateCommand<string> CloseCommand =>
        _closeCommand ?? new DelegateCommand<string>(ExecuteCloseCommand);

    public bool UserInputRequired
    {
        get => _userInputRequired;
        set
        {
            SetProperty(ref _userInputRequired, value);
            RaisePropertyChanged();
        }
    }

    public int Order
    {
        get => _order;
        set
        {
            SetProperty(ref _order, value);
            RaisePropertyChanged();
        }
    }

    public string Command
    {
        get => _command;
        set
        {
            SetProperty(ref _command, value);
            RaisePropertyChanged();
        }
    }

    public bool CanCloseDialog()
    {
        return true;
    }

    public void OnDialogClosed()
    {
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
        
    }

    public event Action<IDialogResult>? RequestClose;

    public string Title { get; }

    private void ExecuteCloseCommand(string ctx)
    {
        switch (ctx)
        {
            case "Save":
                var dialogResults = new DialogParameters
                {
                    { "Command", _command },
                    { "Order", _order },
                    { "RequiresUserInput", _userInputRequired }
                };
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, dialogResults));
                break;
            case "Cancel":
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
                break;
        }
    }
}