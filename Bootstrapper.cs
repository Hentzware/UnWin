using System.Windows;
using Prism.DryIoc;
using Prism.Ioc;
using UnWin.Views;

namespace UnWin;

public class Bootstrapper : PrismBootstrapper
{
    protected override DependencyObject CreateShell()
    {
        return Container.Resolve<ShellView>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
    }
}