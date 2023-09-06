namespace UnWin.Models;

public class LogonCommand
{
    public bool UserInputRequired { get; set; }

    public int Order { get; set; }

    public string Command { get; set; }
}