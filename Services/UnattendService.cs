﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnWin.Models;
using XmlSerializer.Models;

namespace UnWin.Services;

public class UnattendService : IUnattendService
{
    private readonly ISettingsService _settingsService;

    public UnattendService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public void SaveAutounattendXmlFile(string filePath)
    {
        var unattend = CreateAutounattendFile();
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Unattend));
        var types = new Type[] { };
        var namespaces = new XmlSerializerNamespaces();

        namespaces.Add("wcm", "http://schemas.microsoft.com/WMIConfig/2002/State");
        namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");

        using (var stream = new StreamWriter(filePath))
        {
            serializer.Serialize(stream, unattend, namespaces);
        }
    }

    private Unattend CreateAutounattendFile()
    {
        var settings = _settingsService.LoadAutounattendSettings();

        var unattend = new Unattend
        {
            Settings = new List<Settings>()
        };


        // 1. WindowsPE

        var settingWindowsPe = new Settings
        {
            Pass = "windowsPE",
            Components = new List<Component>()
        };

        unattend.Settings.Add(settingWindowsPe);

        var componentWinIntCoreWinPe = new Component
        {
            Name = "Microsoft-Windows-International-Core-WinPE",
            InputLocale = settings.Language,
            SystemLocale = settings.Language,
            UILanguage = settings.Language,
            UserLocale = settings.Language,
            SetupUILanguage = new SetupUILanguage
            {
                UILanguage = settings.Language
            }
        };

        settingWindowsPe.Components.Add(componentWinIntCoreWinPe);

        var componentWinSetup = new Component
        {
            Name = "Microsoft-Windows-Setup",
            DiskConfiguration = new DiskConfiguration
            {
                Disk = new Disk
                {
                    Action = "add",
                    CreatePartitions = new CreatePartitions
                    {
                        CreatePartition = new List<CreatePartition>
                        {
                            new()
                            {
                                Action = "add",
                                Order = 1,
                                Size = settings.EFISize,
                                Type = "EFI"
                            },
                            new()
                            {
                                Action = "add",
                                Order = 2,
                                Size = settings.OSSize,
                                Type = "Primary"
                            },
                            new()
                            {
                                Action = "add",
                                Order = 3,
                                Size = settings.WinRESize,
                                Type = "Primary"
                            }
                        }
                    },
                    ModifyPartitions = new ModifyPartitions
                    {
                        ModifyPartition = new List<ModifyPartition>
                        {
                            new()
                            {
                                Action = "add",
                                Order = 1,
                                Format = "FAT32",
                                Label = "EFI",
                                PartitionID = 1
                            },
                            new()
                            {
                                Action = "add",
                                Order = 2,
                                Format = "NTFS",
                                Label = "OS",
                                PartitionID = 2
                            },
                            new()
                            {
                                Action = "add",
                                Order = 3,
                                Format = "NTFS",
                                Label = "WinRE",
                                PartitionID = 3,
                                TypeID = "de94bba4-06d1-4d40-a16a-bfd50179d6ac"
                            }
                        }
                    },
                    DiskId = 0,
                    WillWipeDisk = true
                }
            },
            ImageInstall = new ImageInstall
            {
                OSImage = new OSImage
                {
                    InstallTo = new InstallTo
                    {
                        DiskID = 0,
                        PartitionID = 2
                    },
                    WillShowUI = "OnError"
                }
            },
            UserData = new UserData
            {
                AcceptEula = true
            }
        };

        settingWindowsPe.Components.Add(componentWinSetup);


        // 4. Specialize

        var settingSpecialize = new Settings
        {
            Pass = "specialize",
            Components = new List<Component>()
        };

        unattend.Settings.Add(settingSpecialize);

        var componentWinShellSetupSpecialize = new Component
        {
            Name = "Microsoft-Windows-Shell-Setup",
            ComputerName = settings.ComputerName,
            TimeZone = "W. European Standard Time"
        };

        settingSpecialize.Components.Add(componentWinShellSetupSpecialize);


        // 7. OOBE System

        var settingOobeSystem = new Settings
        {
            Pass = "oobeSpecialize",
            Components = new List<Component>()
        };

        unattend.Settings.Add(settingOobeSystem);

        var componentWinIntCore = new Component
        {
            Name = "Microsoft-Windows-International-Core",
            InputLocale = settings.Language,
            SystemLocale = settings.Language,
            UILanguage = settings.Language,
            UserLocale = settings.Language
        };

        settingOobeSystem.Components.Add(componentWinIntCore);

        var componentWinShellSetupOobe = new Component
        {
            Name = "Microsoft-Windows-Shell-Setup",
            OOBE = new OOBE
            {
                HideEULAPage = true,
                HideLocalAccountScreen = true,
                HideOEMRegistrationScreen = true,
                HideOnlineAccountScreens = true,
                HideWirelessSetupInOOBE = true,
                ProtectYourPC = 3
            },
            UserAccounts = new UserAccounts()
        };

        if (!string.IsNullOrEmpty(settings.AdministratorPassword))
        {
            componentWinShellSetupOobe.UserAccounts.AdministratorPassword = new Password()
            {
                Value = settings.AdministratorPassword,
                PlainText = true
            };
        }

        if (settings.CreateLocalAccount)
        {
            var localAccount = new LocalAccount
            {
                Action = "add",
                Name = settings.Name,
                DisplayName = settings.DisplayName,
                Group = settings.Group,
                Password = new Password
                {
                    Value = settings.Password,
                    PlainText = true
                }
            };

            componentWinShellSetupOobe.UserAccounts.LocalAccounts = new LocalAccounts
            {
                LocalAccount = localAccount
            };
        }

        if (settings.AutoLogonEnabled)
        {
            var autoLogon = new AutoLogon
            {
                Action = "add",
                Enabled = true,
                LogonCount = settings.AutoLogonCount
            };

            if (settings.CreateLocalAccount)
            {
                autoLogon.Password = new Password
                {
                    Value = settings.Password,
                    PlainText = true
                };

                autoLogon.Username = settings.Name;
            }
            else
            {
                autoLogon.Password = new Password
                {
                    Value = settings.AdministratorPassword,
                    PlainText = true
                };

                autoLogon.Username = "administrator";
            }
        }

        if (settings.FirstLogonCommands.Count > 0)
        {
            var commands = new FirstLogonCommands()
            {
                SynchronousCommand = new List<SynchronousCommand>()
            };

            foreach (var firstLogonCommand in settings.FirstLogonCommands)
            {
                var syncCommand = new SynchronousCommand()
                {
                    Action = "add",
                    Order = firstLogonCommand.Order,
                    CommandLine = firstLogonCommand.Command,
                    RequiresUserInput = firstLogonCommand.RequiresUserInput
                };

                commands.SynchronousCommand.Add(syncCommand);
            }

            componentWinShellSetupOobe.FirstLogonCommands = commands;
        }

        if (settings.LogonCommands.Count > 0)
        {
            var commands = new LogonCommands()
            {
                AsynchronousCommand = new List<AsynchronousCommand>()
            };

            foreach (var logonCommand in settings.LogonCommands)
            {
                var asyncCommand = new AsynchronousCommand()
                {
                    Action = "add",
                    Order = logonCommand.Order,
                    CommandLine = logonCommand.Command,
                    RequiresUserInput = logonCommand.RequiresUserInput
                };

                commands.AsynchronousCommand.Add(asyncCommand);
            }

            componentWinShellSetupOobe.LogonCommands = commands;
        }

        settingOobeSystem.Components.Add(componentWinShellSetupOobe);

        return unattend;
    }
}