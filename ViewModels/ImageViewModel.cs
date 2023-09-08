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
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using UnWin.Models;
using UnWin.Services;
using UnWin.Views;

namespace UnWin.ViewModels;

public class ImageViewModel : BindableBase
{
    private readonly IRegionManager _regionManager;
    private readonly ISettingsService _settingsService;
    private readonly IUnattendService _unattendService;
    private bool _isAutounattendConfigEnabled;
    private bool _isAutounattendImportEnabled;
    private bool _isCreateImageEnabled;
    private DelegateCommand _createCommand;
    private DelegateCommand _openUnattendConfigCommand;
    private DelegateCommand<string> _openCommand;
    private DelegateCommand<TextBox> _onTextChanged;
    private int _autounattendMode;
    private string _autounattendPath;
    private string _efisysBinPath;
    private string _extractionPath;
    private string _log;
    private string _oscdimgPath;
    private string _sourceIsoPath;
    private string _targetIsoPath;

    public ImageViewModel(IRegionManager regionManager, ISettingsService settingsService, IEventAggregator eventAggregator, IUnattendService unattendService)
    {
        _regionManager = regionManager;
        _settingsService = settingsService;
        _unattendService = unattendService;

        LoadSettings();
    }

    public DelegateCommand CreateCommand =>
        _createCommand ?? new DelegateCommand(ExecuteCreateCommand);

    public DelegateCommand<TextBox> OnTextChanged =>
        _onTextChanged ?? new DelegateCommand<TextBox>(ExecuteOnTextChanged);

    public DelegateCommand<string> OpenCommand =>
        _openCommand ?? new DelegateCommand<string>(ExecuteOpenCommand);

    public DelegateCommand OpenUnattendConfigCommand =>
        _openUnattendConfigCommand ?? new DelegateCommand(ExecuteOpenUnattendConfigCommand);

    public bool IsAutounattendConfigEnabled => _autounattendMode == 1;

    public bool IsAutounattendImportEnabled => _autounattendMode == 0;

    public bool IsCreateImageEnabled
    {
        get
        {
            if ((_autounattendMode == 0 && string.IsNullOrEmpty(_autounattendPath)) ||
                string.IsNullOrEmpty(_oscdimgPath) ||
                string.IsNullOrEmpty(_sourceIsoPath) ||
                string.IsNullOrEmpty(_extractionPath) ||
                string.IsNullOrEmpty(_targetIsoPath))
            {
                return false;
            }

            return true;
        }
    }

    public int AutounattendMode
    {
        get => _autounattendMode;
        set
        {
            SetProperty(ref _autounattendMode, value);
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(IsAutounattendConfigEnabled));
            RaisePropertyChanged(nameof(IsAutounattendImportEnabled));
            RaisePropertyChanged(nameof(IsCreateImageEnabled));
            SaveSettings();
        }
    }

    public string AutounattendPath
    {
        get => _autounattendPath;
        set
        {
            SetProperty(ref _autounattendPath, value);
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(IsCreateImageEnabled));
            SaveSettings();
        }
    }

    public string ExtractionPath
    {
        get => _extractionPath;
        set
        {
            SetProperty(ref _extractionPath, value);
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(IsCreateImageEnabled));
            SaveSettings();
        }
    }

    public string Log
    {
        get => _log;
        set
        {
            SetProperty(ref _log, value);
            RaisePropertyChanged();
        }
    }

    public string OscdimgPath
    {
        get => _oscdimgPath;
        set
        {
            SetProperty(ref _oscdimgPath, value);
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(IsCreateImageEnabled));
            SaveSettings();
        }
    }

    public string SourceIsoPath
    {
        get => _sourceIsoPath;
        set
        {
            SetProperty(ref _sourceIsoPath, value);
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(IsCreateImageEnabled));
            SaveSettings();
        }
    }

    public string TargetIsoPath
    {
        get => _targetIsoPath;
        set
        {
            SetProperty(ref _targetIsoPath, value);
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(IsCreateImageEnabled));
            SaveSettings();
        }
    }

    private void ApplyUnattend()
    {
        var xml3 = Path.Combine(_extractionPath, "autounattend.xml");

        if (File.Exists(xml3))
        {
            File.Delete(xml3);
        }

        if (_autounattendMode == 1)
        {
            _unattendService.SaveAutounattendXmlFile(xml3);
            return;
        }

        File.Copy(_autounattendPath, xml3);
    }

    private async Task CreateImage()
    {
        await RunCommand($"oscdimg.exe -m -o -h -u2 -udfver102 -b\"{_efisysBinPath}\" -pEF \"{_extractionPath}\" \"{_targetIsoPath}\"");
    }

    private async void ExecuteCreateCommand()
    {
        await ExtractIso();
        SearchEFIBootFile();
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
            case nameof(OscdimgPath):
                OscdimgPath = SelectFolder(openFolderDialog);
                break;

            case nameof(AutounattendPath):
                openFileDialog.Filter = "XML|*.xml";
                AutounattendPath = SelectFile(openFileDialog);
                break;

            case nameof(SourceIsoPath):
                openFileDialog.Filter = "ISO|*.iso";
                SourceIsoPath = SelectFile(openFileDialog);
                break;

            case nameof(ExtractionPath):
                ExtractionPath = SelectFolder(openFolderDialog);
                break;

            case nameof(TargetIsoPath):
                saveFileDialog.Filter = "ISO|*.iso";
                TargetIsoPath = SaveFile(saveFileDialog);
                break;
        }
    }

    private void ExecuteOpenUnattendConfigCommand()
    {
        _regionManager.RequestNavigate("ContentRegion", nameof(UnattendView));
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

        foreach (var dir in directory.GetDirectories())
        {
            await ExtractDirectory(dir, Path.Combine(outputPath, dir.Name));
        }
    }

    private async Task ExtractIso()
    {
        if (Directory.GetFiles(_extractionPath).Length > 0)
        {
            return;
        }

        using (var isoStream = File.OpenRead(_sourceIsoPath))
        {
            var udf = new UdfReader(isoStream);
            await ExtractDirectory(udf.Root, _extractionPath);
        }
    }

    private void LoadSettings()
    {
        var imageSettings = _settingsService.LoadImageSettings();

        OscdimgPath = imageSettings.OscdimgPath;
        SourceIsoPath = imageSettings.SourceIsoPath;
        TargetIsoPath = imageSettings.TargetIsoPath;
        AutounattendPath = imageSettings.AutounattendPath;
        AutounattendMode = imageSettings.AutounattendMode;
        ExtractionPath = imageSettings.ExtractionPath;
    }

    private void RemoveBootPrompt()
    {
        var cdbootEfiPrompt = Directory.GetFiles(_extractionPath, "cdboot_prompt.efi", SearchOption.AllDirectories)
            .FirstOrDefault();
        var cdbootEfi = Directory.GetFiles(_extractionPath, "cdboot.efi", SearchOption.AllDirectories)
            .FirstOrDefault();
        var cdbootEfiNoPrompt = Directory
            .GetFiles(_extractionPath, "cdboot_noprompt.efi", SearchOption.AllDirectories).FirstOrDefault();
        var efisysBin = Directory.GetFiles(_extractionPath, "efisys.bin", SearchOption.AllDirectories)
            .FirstOrDefault();
        var efisysBinNoPrompt = Directory
            .GetFiles(_extractionPath, "efisys_noprompt.bin", SearchOption.AllDirectories).FirstOrDefault();
        var directory = cdbootEfi.Replace("cdboot.efi", "");


        if (!string.IsNullOrEmpty(cdbootEfiPrompt))
        {
            return;
        }

        if (!string.IsNullOrEmpty(cdbootEfi))
        {
            File.Move(cdbootEfi, $"{directory}cdboot_prompt.efi");
        }

        if (!string.IsNullOrEmpty(cdbootEfiNoPrompt))
        {
            File.Move(cdbootEfiNoPrompt, $"{directory}cdboot.efi");
        }

        if (!string.IsNullOrEmpty(efisysBin))
        {
            File.Move(efisysBin, $"{directory}efisys_prompt.bin");
        }

        if (!string.IsNullOrEmpty(efisysBinNoPrompt))
        {
            File.Move(efisysBinNoPrompt, $"{directory}efisys.bin");
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
            WorkingDirectory = _oscdimgPath
        };

        using var process = new Process { StartInfo = cmd, EnableRaisingEvents = true };

        process.OutputDataReceived += (sender, e) =>
        {
            if (e.Data != null)
            {
                Log += e.Data + "\n";
            }
        };

        process.Start();

        using (var streamWriter = process.StandardInput)
        {
            if (streamWriter.BaseStream.CanWrite)
            {
                streamWriter.WriteLine(command);
            }
        }

        process.BeginOutputReadLine();

        await process.WaitForExitAsync();

        if (!process.HasExited)
        {
            process.Kill();
        }
    }

    private string SaveFile(SaveFileDialog saveFileDialog)
    {
        if (saveFileDialog.ShowDialog() == true)
        {
            return saveFileDialog.FileName;
        }

        return string.Empty;
    }

    private void SaveSettings()
    {
        var imageSettings = new ImageSettings
        {
            OscdimgPath = _oscdimgPath,
            SourceIsoPath = _sourceIsoPath,
            TargetIsoPath = _targetIsoPath,
            AutounattendPath = _autounattendPath,
            AutounattendMode = _autounattendMode,
            ExtractionPath = _extractionPath
        };

        _settingsService.SaveImageSettings(imageSettings);
    }

    private void SearchEFIBootFile()
    {
        _efisysBinPath = Directory.GetFiles(_extractionPath, "efisys.bin", SearchOption.AllDirectories).First();
    }

    private string SelectFile(OpenFileDialog openFileDialog)
    {
        if (openFileDialog.ShowDialog() == true)
        {
            return openFileDialog.FileName;
        }

        return string.Empty;
    }

    private string SelectFolder(VistaFolderBrowserDialog openFolderDialog)
    {
        if (openFolderDialog.ShowDialog() == true)
        {
            return openFolderDialog.SelectedPath;
        }

        return "";
    }
}