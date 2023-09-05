using Prism.Mvvm;
using Prism.Regions;
using UnWin.Views;

namespace UnWin.ViewModels;

public class ShellViewModel : BindableBase
{
    public ShellViewModel(IRegionManager regionManager)
    {
        regionManager.RegisterViewWithRegion<ImageView>("ContentRegion");
    }
}