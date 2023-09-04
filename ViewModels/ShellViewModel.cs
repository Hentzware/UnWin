using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using DiscUtils;
using DiscUtils.Udf;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using UnWin.Properties;

namespace UnWin.ViewModels;

public class ShellViewModel : BindableBase
{
    private DelegateCommand _createCommand;
    private DelegateCommand<string> _openCommand;
    private DelegateCommand<TextBox> _onTextChanged;
    private string _efisysBinPath;
    private string _etfsbootComPath;
    private string _log;
    private string _oscdimgExeDirPath;
    private string _sourceIsoDirPath;
    private string _sourceIsoPath;
    private string _targetIsoPath;
    private string _unattendXmlPath;

    public ShellViewModel()
    {
        TargetIsoPath = Settings.Default.TargetIsoPath;
        UnattendXmlPath = Settings.Default.UnattendXmlPath;
        OscdimgExeDirPath = Settings.Default.OscdimgExeDirPath;
        SourceIsoPath = Settings.Default.SourceIsoPath;
        SourceIsoDirPath = Settings.Default.SourceIsoDirPath;
    }

    public DelegateCommand CreateCommand =>
        _createCommand ?? new DelegateCommand(ExecuteCreateCommand);

    public DelegateCommand<string> OpenCommand =>
        _openCommand ?? new DelegateCommand<string>(ExecuteOpenCommand);

    public DelegateCommand<TextBox> OnTextChanged =>
        _onTextChanged ?? new DelegateCommand<TextBox>(ExecuteOnTextChanged);

    public string Log
    {
        get => _log;
        set
        {
            SetProperty(ref _log, value);
            RaisePropertyChanged();
        }
    }

    public string OscdimgExeDirPath
    {
        get => _oscdimgExeDirPath;
        set
        {
            SetProperty(ref _oscdimgExeDirPath, value);
            Settings.Default.OscdimgExeDirPath = value;
            Settings.Default.Save();
            RaisePropertyChanged();
        }
    }

    public string SourceIsoDirPath
    {
        get => _sourceIsoDirPath;
        set
        {
            SetProperty(ref _sourceIsoDirPath, value);
            Settings.Default.SourceIsoDirPath = value;
            Settings.Default.Save();
            RaisePropertyChanged();
        }
    }

    public string SourceIsoPath
    {
        get => _sourceIsoPath;
        set
        {
            SetProperty(ref _sourceIsoPath, value);
            Settings.Default.SourceIsoPath = value;
            Settings.Default.Save();
            RaisePropertyChanged();
        }
    }

    public string TargetIsoPath
    {
        get => _targetIsoPath;
        set
        {
            SetProperty(ref _targetIsoPath, value);
            Settings.Default.TargetIsoPath = value;
            Settings.Default.Save();
            RaisePropertyChanged();
        }
    }

    public string UnattendXmlPath
    {
        get => _unattendXmlPath;
        set
        {
            SetProperty(ref _unattendXmlPath, value);
            Settings.Default.UnattendXmlPath = value;
            Settings.Default.Save();
            RaisePropertyChanged();
        }
    }

    private string SaveFile(SaveFileDialog saveFileDialog)
    {
        if (saveFileDialog.ShowDialog() == true) return saveFileDialog.FileName;

        return string.Empty;
    }

    private string SelectFile(OpenFileDialog openFileDialog)
    {
        if (openFileDialog.ShowDialog() == true) return openFileDialog.FileName;

        return string.Empty;
    }

    private string SelectFolder(VistaFolderBrowserDialog openFolderDialog)
    {
        if (openFolderDialog.ShowDialog() == true) return openFolderDialog.SelectedPath;

        return "";
    }

    private async Task CreateImage()
    {
        await RunCommand($"oscdimg.exe -m -o -h -u2 -udfver102 -b\"{_efisysBinPath}\" -pEF \"{_sourceIsoDirPath}\" \"{_targetIsoPath}\"");
    }

    private async Task ExtractDirectory(DiscDirectoryInfo directory, string outputPath)
    {
        foreach (var file in directory.GetFiles())
        {
            var newFilePath = Path.Combine(outputPath, file.Name);

            Directory.CreateDirectory(Path.GetDirectoryName(newFilePath));

            using (var fileStream = file.OpenRead())

            using (var outputStream = File.Create(newFilePath))
            {
                Log += $"Kopiere {file.Name}" + "\n";
                await fileStream.CopyToAsync(outputStream);
            }
        }

        foreach (var dir in directory.GetDirectories()) await ExtractDirectory(dir, Path.Combine(outputPath, dir.Name));
    }

    private async Task ExtractIso()
    {
        using (var isoStream = File.OpenRead(_sourceIsoPath))
        {
            var udf = new UdfReader(isoStream);
            await ExtractDirectory(udf.Root, _sourceIsoDirPath);
        }
    }

    private async Task RunCommand(string command)
    {
        var cmd = new ProcessStartInfo("cmd.exe")
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = _oscdimgExeDirPath
        };

        using var process = new Process { StartInfo = cmd, EnableRaisingEvents = true };

        process.OutputDataReceived += (sender, e) =>
        {
            if (e.Data != null) Log += e.Data + "\n";
        };

        process.Start();

        using (var streamWriter = process.StandardInput)
        {
            if (streamWriter.BaseStream.CanWrite) streamWriter.WriteLine(command);
        }

        process.BeginOutputReadLine();

        await process.WaitForExitAsync();

        if (!process.HasExited) process.Kill();
    }

    private void ApplyUnattend()
    {
        var xml3 = Path.Combine(_sourceIsoDirPath, "autounattend.xml");

        if (File.Exists(xml3)) File.Delete(xml3);

        File.Copy(_unattendXmlPath, xml3);
    }

    private async void ExecuteCreateCommand()
    {
        if (Directory.GetFiles(_sourceIsoDirPath).Length == 0) await ExtractIso();

        SearchFiles();
        RemoveBootPrompt();
        ApplyUnattend();
        await CreateImage();
    }

    private void ExecuteOnTextChanged(TextBox textBox)
    {
        textBox.ScrollToEnd();
    }

    private void ExecuteOpenCommand(string ctx)
    {
        var openFileDialog = new OpenFileDialog();
        var saveFileDialog = new SaveFileDialog();
        var openFolderDialog = new VistaFolderBrowserDialog();

        switch (ctx)
        {
            case nameof(OscdimgExeDirPath):
                OscdimgExeDirPath = SelectFolder(openFolderDialog);
                break;

            case nameof(UnattendXmlPath):
                openFileDialog.Filter = "XML|*.xml";
                UnattendXmlPath = SelectFile(openFileDialog);
                break;

            case nameof(SourceIsoPath):
                openFileDialog.Filter = "ISO|*.iso";
                SourceIsoPath = SelectFile(openFileDialog);
                break;

            case nameof(SourceIsoDirPath):
                SourceIsoDirPath = SelectFolder(openFolderDialog);
                break;

            case nameof(TargetIsoPath):
                saveFileDialog.Filter = "ISO|*.iso";
                TargetIsoPath = SaveFile(saveFileDialog);
                break;
        }
    }

    private void RemoveBootPrompt()
    {
        var cdbootEfiPrompt = Directory.GetFiles(_sourceIsoDirPath, "cdboot_prompt.efi", SearchOption.AllDirectories)
            .FirstOrDefault();
        var cdbootEfi = Directory.GetFiles(_sourceIsoDirPath, "cdboot.efi", SearchOption.AllDirectories)
            .FirstOrDefault();
        var cdbootEfiNoPrompt = Directory
            .GetFiles(_sourceIsoDirPath, "cdboot_noprompt.efi", SearchOption.AllDirectories).FirstOrDefault();
        var efisysBin = Directory.GetFiles(_sourceIsoDirPath, "efisys.bin", SearchOption.AllDirectories)
            .FirstOrDefault();
        var efisysBinNoPrompt = Directory
            .GetFiles(_sourceIsoDirPath, "efisys_noprompt.bin", SearchOption.AllDirectories).FirstOrDefault();
        var directory = cdbootEfi.Replace("cdboot.efi", "");


        if (!string.IsNullOrEmpty(cdbootEfiPrompt)) return;

        if (!string.IsNullOrEmpty(cdbootEfi)) File.Move(cdbootEfi, $"{directory}cdboot_prompt.efi");

        if (!string.IsNullOrEmpty(cdbootEfiNoPrompt)) File.Move(cdbootEfiNoPrompt, $"{directory}cdboot.efi");

        if (!string.IsNullOrEmpty(efisysBin)) File.Move(efisysBin, $"{directory}efisys_prompt.bin");

        if (!string.IsNullOrEmpty(efisysBinNoPrompt)) File.Move(efisysBinNoPrompt, $"{directory}efisys.bin");
    }

    private void SearchFiles()
    {
        _efisysBinPath = Directory.GetFiles(_sourceIsoDirPath, "efisys.bin", SearchOption.AllDirectories).First();
        _etfsbootComPath = Directory.GetFiles(_sourceIsoDirPath, "etfsboot.com", SearchOption.AllDirectories).First();
    }
}