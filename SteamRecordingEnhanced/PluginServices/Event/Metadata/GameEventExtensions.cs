using System.Linq;

namespace SteamRecordingEnhanced.PluginServices.Event.Metadata;

public static class GameEventExtensions
{
    extension(GameEvent value)
    {
        public string GetDefaultIcon() => GetMetadataAttribute(value)?.DefaultIcon ?? "steam_none";
        public string GetLabel() => GetMetadataAttribute(value)?.Label ?? "";
        public string? GetDescription() => GetMetadataAttribute(value)?.Description;
    }

    private static GameEventMetadataAttribute? GetMetadataAttribute(GameEvent value)
    {
        return value.GetType()?
            .GetField(value.ToString())?
            .GetCustomAttributes(typeof(GameEventMetadataAttribute), false)
            .SingleOrDefault() as GameEventMetadataAttribute;
    }
}