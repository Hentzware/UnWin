using System.Windows;

namespace UnWin;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var bootstrapper = new Bootstrapper();
        bootstrapper.Run();

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
    }
}