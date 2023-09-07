using System.Collections.Generic;

namespace UnWin.Models;

public class AutounattendSettings
{
    public bool AutoLogonEnabled { get; set; }

    public int AutoLogonCount { get; set; }

    public int EFISize { get; set; }

    public int OSSize { get; set; }

    public int VersionIndex { get; set; }

    public int WinRESize { get; set; }

    public List<LogonCommand> FirstLogonCommands { get; set; }

    public List<LogonCommand> LogonCommands { get; set; }

    public string ComputerName { get; set; }

    public string Language { get; set; }

    public string UserName { get; set; }
}