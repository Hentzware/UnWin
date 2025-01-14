﻿namespace UnWin.Models;

public class ImageSettings
{
    public bool Sysprep { get; set; }

    public int AutounattendMode { get; set; }

    public string AutounattendPath { get; set; }

    public string ExtractionPath { get; set; }

    public string OscdimgPath { get; set; }

    public string SourceIsoPath { get; set; }

    public string TargetIsoPath { get; set; }
}