using System;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.Windows.Tabs;

internal class SettingsTab : ITab
{
    public string Title => "Settings";

    public void Draw()
    {
        ImGui.TextUnformatted("Event icons");
        SettingsSteamIconSelect($"Level up", ref Services.Configuration.LevelUpIcon);
        SettingsSteamIconSelect($"Quest complete", ref Services.Configuration.QuestCompleteIcon);
        SettingsSteamIconSelect($"Achievement unlocked", ref Services.Configuration.AchievementUnlockedIcon);
        SettingsSteamIconSelect($"Territory changed", ref Services.Configuration.TerritoryChangedIcon);
        SettingsSteamIconSelect($"Duty started", ref Services.Configuration.DutyStartedIcon);
        SettingsSteamIconSelect($"Duty wiped", ref Services.Configuration.DutyWipedIcon);
        SettingsSteamIconSelect($"Duty complete", ref Services.Configuration.DutyCompleteIcon);
        SettingsSteamIconSelect($"Player died", ref Services.Configuration.PlayerDiedIcon);
        SettingsSteamIconSelect($"Party member died", ref Services.Configuration.PartyMemberDiedIcon);
        ImGui.Separator();
        ImGui.TextUnformatted("Highlighted events");
        SettingCheckbox("Combat", ref Services.Configuration.HighlightCombat);
    }

    private bool SettingCheckbox(string label, ref bool value, bool save = true)
    {
        if (ImGui.Checkbox(label, ref value))
        {
            if (save) Services.Configuration.Save();
            return true;
        }

        return false;
    }

    private bool SettingsSteamIconSelect(string label, ref string value)
    {
        if (GuiUtils.SteamIconSelect(label, ref value))
        {
            Services.Configuration.Save();
            return true;
        }

        return false;
    }
}
