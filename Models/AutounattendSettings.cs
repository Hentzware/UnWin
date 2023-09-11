using System.Collections.Generic;

namespace UnWin.Models;

public class AutounattendSettings
{
    public bool AutoLogonEnabled { get; set; }

    public bool CreateLocalAccount { get; set; }

    public bool VersionIndexEnabled { get; set; }

    public int AutoLogonCount { get; set; }

    public int EFISize { get; set; }

    public int OSSize { get; set; }

    public int VersionIndex { get; set; }

    public int WinRESize { get; set; }

    public List<LogonCommand> FirstLogonCommands { get; set; }

    public List<LogonCommand> LogonCommands { get; set; }

    public string AdministratorPassword { get; set; }

    public string ComputerName { get; set; }

    public string DisplayName { get; set; }

    public string Group { get; set; }

    public string Language { get; set; }

    public string Name { get; set; }

    public string Password { get; set; }
}