using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace UnWin.ViewModels
{
    public class LogonCommandDialogViewModel : BindableBase, IDialogAware
    {
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

        public string Title { get; }

        public event Action<IDialogResult>? RequestClose;
    }
}
