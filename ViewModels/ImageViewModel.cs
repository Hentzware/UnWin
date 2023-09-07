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
using UnWin.PubSubEvents;
using UnWin.Services;
using UnWin.Views;

namespace UnWin.ViewModels;

public class ImageViewModel : BindableBase
{
    private readonly IRegionManager _regionManager;
    private readonly ISettingsService _settingsService;
    private readonly IEventAggregator _eventAggregator;
    private DelegateCommand _createCommand;
    private DelegateCommand _openUnattendConfigCommand;
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
    private Settings _settings;

    public ImageViewModel(IRegionManager regionManager, ISettingsService settingsService, IEventAggregator eventAggregator)
    {
        _regionManager = regionManager;
        _settingsService = settingsService;
        _eventAggregator = eventAggregator;

        _eventAggregator.GetEvent<CloseEvent>().Subscribe(OnAppClosing);
        _settings = _settingsService.Load();

        OscdimgExeDirPath = _settings.OscdimgPath;
        SourceIsoPath = _settings.SourceIsoPath;
        SourceIsoDirPath = _settings.SourceIsoPath;
        TargetIsoPath = _settings.TargetIsoPath;
        UnattendXmlPath = _settings.AutounattendPath;
    }

    private void OnAppClosing()
    {
        _settingsService.Save(_settings);
    }


    public DelegateCommand CreateCommand =>
        _createCommand ?? new DelegateCommand(ExecuteCreateCommand);

    public DelegateCommand<TextBox> OnTextChanged =>
        _onTextChanged ?? new DelegateCommand<TextBox>(ExecuteOnTextChanged);

    public DelegateCommand<string> OpenCommand =>
        _openCommand ?? new DelegateCommand<string>(ExecuteOpenCommand);

    public DelegateCommand OpenUnattendConfigCommand =>
        _openUnattendConfigCommand ?? new DelegateCommand(ExecuteOpenUnattendConfigCommand);

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
            _settings.OscdimgPath = value;
            RaisePropertyChanged();
        }
    }

    public string SourceIsoDirPath
    {
        get => _sourceIsoDirPath;
        set
        {
            SetProperty(ref _sourceIsoDirPath, value);
            RaisePropertyChanged();
        }
    }

    public string SourceIsoPath
    {
        get => _sourceIsoPath;
        set
        {
            SetProperty(ref _sourceIsoPath, value);
            _settings.SourceIsoPath = value;
            RaisePropertyChanged();
        }
    }

    public string TargetIsoPath
    {
        get => _targetIsoPath;
        set
        {
            SetProperty(ref _targetIsoPath, value);
            _settings.TargetIsoPath = value;
            RaisePropertyChanged();
        }
    }

    public string UnattendXmlPath
    {
        get => _unattendXmlPath;
        set
        {
            SetProperty(ref _unattendXmlPath, value);
            _settings.AutounattendPath = value;
            RaisePropertyChanged();
        }
    }

    private void ApplyUnattend()
    {
        var xml3 = Path.Combine(_sourceIsoDirPath, "autounattend.xml");

        if (File.Exists(xml3))
        {
            File.Delete(xml3);
        }

        File.Copy(_unattendXmlPath, xml3);
    }

    private async Task CreateImage()
    {
        await RunCommand($"oscdimg.exe -m -o -h -u2 -udfver102 -b\"{_efisysBinPath}\" -pEF \"{_sourceIsoDirPath}\" \"{_targetIsoPath}\"");
    }

    private async void ExecuteCreateCommand()
    {
        await ExtractIso();
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
        if (Directory.GetFiles(_sourceIsoDirPath).Length > 0)
        {
            return;
        }

        using (var isoStream = File.OpenRead(_sourceIsoPath))
        {
            var udf = new UdfReader(isoStream);
            await ExtractDirectory(udf.Root, _sourceIsoDirPath);
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
            WorkingDirectory = _oscdimgExeDirPath
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

    private void SearchFiles()
    {
        _efisysBinPath = Directory.GetFiles(_sourceIsoDirPath, "efisys.bin", SearchOption.AllDirectories).First();
        _etfsbootComPath = Directory.GetFiles(_sourceIsoDirPath, "etfsboot.com", SearchOption.AllDirectories).First();
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