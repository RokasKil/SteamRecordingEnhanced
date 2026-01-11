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
        ImGuiComponents.HelpMarker("Works with Eureka and Occult Crescent leveling systems.");
        SettingsSteamIconSelect($"Quest completed", ref Services.Configuration.QuestCompleteIcon);
        SettingsSteamIconSelect($"Achievement unlocked", ref Services.Configuration.AchievementUnlockedIcon);
        SettingsSteamIconSelect($"Fate completed", ref Services.Configuration.FateCompleteIcon);
        ImGuiComponents.HelpMarker("Works with Eureka, Bozja and Occult Crescent fates and critical engagements.");
        SettingsSteamIconSelect($"Territory changed", ref Services.Configuration.TerritoryChangedIcon);
        SettingsSteamIconSelect($"Duty started", ref Services.Configuration.DutyStartedIcon);
        SettingsSteamIconSelect($"Duty wiped", ref Services.Configuration.DutyWipedIcon);
        SettingsSteamIconSelect($"Duty completed", ref Services.Configuration.DutyCompleteIcon);
        SettingsSteamIconSelect($"Player died", ref Services.Configuration.PlayerDiedIcon);
        SettingsSteamIconSelect($"Party member died", ref Services.Configuration.PartyMemberDiedIcon);
        SettingsSteamIconSelect($"PVP kill", ref Services.Configuration.PvpKillIcon);
        ImGuiComponents.HelpMarker("Works by reading the combat log which is known to not be 100% accurate" +
                                   " but it will works most of the time.");

        ImGui.Separator();
        ImGui.TextUnformatted("Highlighted events");
        SettingCheckbox("Combat", ref Services.Configuration.HighlightCombat);

        ImGui.Separator();
        ImGui.TextUnformatted("Sessions");
        SettingCheckbox("Start sessions only in instances", ref Services.Configuration.SessionsOnlyInInstance);
        if (ImGui.CollapsingHeader("What are sessions?"))
        {
            using var textWrapPos = ImRaii.TextWrapPos(ImGui.GetFontSize() * 24);
            ImGui.TextWrapped("Steam allows games to split the recording into different sections with their" +
                              " own label and display them in the Sessions View." +
                              " By default this plugin will start a new session each time you load into a new zone," +
                              " you can configure it to only start a new session when you enter an instance." +
                              " Each session will be labeled and can be filtered by the character and zone or instance name." +
                              " You can find the Sessions View by going to Recordings & Screenshots" +
                              " and clicking on the 3 vertical lines icon in the top left of the window or click" +
                              " the button below to open Session View in steam overlay.");
            using var disabled = ImRaii.Disabled(Services.SteamService.IsOverlayEnabled() != true);
            if (ImGui.Button("Open Session View"))
            {
                Services.TimelineService.OpenOverlayToGamePhase();
            }

            if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled) && disabled.Success)
            {
                using (ImRaii.Enabled())
                    ImGui.SetTooltip("Steam Overlay must be enabled!");
            }
        }
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