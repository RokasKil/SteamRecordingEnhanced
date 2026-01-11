using System;

namespace SteamRecordingEnhanced.PluginServices.Event.Metadata;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class GameEventMetadataAttribute(string defaultIcon, string label, string? description = null) : Attribute
{
    public string DefaultIcon { get; init; } = defaultIcon;
    public string Label { get; init; } = label;
    public string? Description { get; init; } = description;
}