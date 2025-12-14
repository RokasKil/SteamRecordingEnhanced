using System;
using Dalamud.Configuration;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public string LevelUpIcon = "steam_effect";
    public string QuestCompleteIcon = "steam_ribbon";
    public string AchievementUnlockedIcon = "steam_achievement";
    public string TerritoryChangedIcon = "steam_transfer";
    public string DutyStartedIcon = "steam_attack";
    public string DutyWipedIcon = "steam_x";
    public string DutyCompleteIcon = "steam_chest";
    public string PlayerDiedIcon = "steam_death";
    public string PartyMemberDiedIcon = "steam_death";

    public bool HighlightCombat = true;

    public int Version { get; set; } = 0;

    public void Save()
    {
        Services.PluginInterface.SavePluginConfig(this);
    }
}
