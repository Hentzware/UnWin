namespace UnWin.Services;

public interface IUnattendService
{
    void SaveAutounattendFile(string filePath);

    void SaveSysprepFile(string filePath);
}