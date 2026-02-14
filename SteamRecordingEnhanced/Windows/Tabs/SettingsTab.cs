using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using SteamRecordingEnhanced.PluginServices.Event.Metadata;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.Windows.Tabs;

internal class SettingsTab : ITab
{
    public string Title => "Settings";

    public void Draw()
    {
        ImGui.Text("Event icons");
        ImGuiComponents.HelpMarker("These are icons that will be placed on your recording timeline." +
                                   " In cases where multiple events happen close to each other" +
                                   " (for example Quest completed and Level up) events higher on the list" +
                                   " will be displayed on top and you'll need to zoom in to see the other events," +
                                   " use the arrows to reorder them to your liking." +
                                   "\nIf you want to disable an event just set it's marker to None.");
        DrawGameEventList();
        ImGui.Separator();
        ImGui.Text("Highlighted timeline events");
        ImGuiComponents.HelpMarker("These are events that have a start and an end, that range will be" +
                                   " highlighted with a yellow line in your timeline.");
        SettingCheckbox("Combat", ref Services.Configuration.HighlightCombat);

        ImGui.Separator();
        ImGui.Text("Sessions");
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

    private void DrawGameEventList()
    {
        var changed = false;
        int? indexToShift = null;
        int offset = 0;
        for (int i = Services.Configuration.GameEventPriorityList.Count - 1; i >= 0; i--)
        {
            using var id = ImRaii.PushId(i);
            using (ImRaii.PushFont(UiBuilder.IconFont))
            {
                using (ImRaii.Disabled(i == Services.Configuration.GameEventPriorityList.Count - 1))
                {
                    if (ImGui.Button(FontAwesomeIcon.AngleUp.ToIconString()))
                    {
                        indexToShift = i;
                        offset = 1;
                    }
                }

                ImGui.SameLine();
                using (ImRaii.Disabled(i == 0))
                {
                    if (ImGui.Button(FontAwesomeIcon.AngleDown.ToIconString()))
                    {
                        indexToShift = i;
                        offset = -1;
                    }
                }

                ImGui.SameLine();
            }

            var gameEvent = Services.Configuration.GameEventPriorityList[i];
            var icon = Services.Configuration.GameEventIconMap[gameEvent];
            if (GuiUtils.SteamIconSelect(gameEvent.GetLabel(), ref icon))
            {
                Services.Configuration.GameEventIconMap[gameEvent] = icon;
                changed = true;
            }

            if (gameEvent.GetDescription() is { } description)
            {
                ImGuiComponents.HelpMarker(description);
            }
        }

        if (indexToShift != null)
        {
            var gameEvent = Services.Configuration.GameEventPriorityList[indexToShift.Value];
            Services.Configuration.GameEventPriorityList.RemoveAt(indexToShift.Value);
            Services.Configuration.GameEventPriorityList.Insert(indexToShift.Value + offset, gameEvent);
            changed = true;
        }

        if (changed)
        {
            Services.Configuration.Save();
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