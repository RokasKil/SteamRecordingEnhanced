using System;
using System.Collections.Generic;
using Dalamud.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SteamRecordingEnhanced.PluginServices.Event.Metadata;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced;

[Serializable]
public class Configuration : IPluginConfiguration
{
    [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
    public List<GameEvent> GameEventPriorityList = [];
    public Dictionary<GameEvent, string> GameEventIconMap = [];
    public bool HighlightCombat = true;

    public bool SessionsOnlyInInstance = false;

    public int Version { get; set; } = 0;

    public void Save()
    {
        Services.PluginInterface.SavePluginConfig(this);
    }

    public void SetDefaults()
    {
        var index = 0;
        foreach (var gameEvent in Enum.GetValues<GameEvent>())
        {
            if (!GameEventPriorityList.Contains(gameEvent))
            {
                GameEventPriorityList.Insert(index, gameEvent);
            }

            if (!GameEventIconMap.ContainsKey(gameEvent))
            {
                GameEventIconMap.Add(gameEvent, gameEvent.GetDefaultIcon());
            }

            index++;
        }
    }
}