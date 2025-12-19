using System;

namespace SteamRecordingEnhanced.Utility;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class IconUrlAttribute(string url) : Attribute
{
    public string Url { get; init; } = url;
}
