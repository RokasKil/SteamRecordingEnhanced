using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.Windows.Tabs;

public class StatusTab : ITab
{
    public string Title => "Status";

    public void Draw()
    {
        using var textWrapPos = ImRaii.TextWrapPos(ImGui.GetFontSize() * 28);
        ImGui.TextUnformatted("Steam loaded:");
        ImGui.SameLine();
        if (Services.SteamService.SteamLoaded)
        {
            DrawOk();
        }
        else
        {
            DrawFail();
            DrawSteamInstructions();
            return;
        }

        ImGui.TextUnformatted("Overlay enabled:");
        ImGui.SameLine();
        var overlayEnabled = Services.SteamService.IsOverlayEnabled();
        if (overlayEnabled.HasValue && overlayEnabled.Value)
        {
            DrawOk();
        }
        else
        {
            DrawFail();
            if (overlayEnabled.HasValue)
            {
                ImGui.TextWrapped("Steam overlay is disabled, the recording feature should still work without it.");
            }
            else
            {
                // Surely this never happens
                ImGui.TextWrapped("Failed to get steam overlay status, something went wrong contact the developer.");
            }

            return;
        }

        ImGui.TextWrapped("Everything seems to be working.");
    }

    private void DrawOk()
    {
        using var color = ImRaii.PushColor(ImGuiCol.Text, new Vector4(0, 1, 0, 1));
        ImGui.TextUnformatted("Ok");
        ImGui.SameLine();
        using var font = ImRaii.PushFont(UiBuilder.IconFont);
        ImGui.TextUnformatted(FontAwesomeIcon.Check.ToIconString());
    }

    private void DrawFail()
    {
        using var color = ImRaii.PushColor(ImGuiCol.Text, new Vector4(1, 0, 0, 1));
        ImGui.TextUnformatted("Fail");
        ImGui.SameLine();
        using var font = ImRaii.PushFont(UiBuilder.IconFont);
        ImGui.TextUnformatted(FontAwesomeIcon.Times.ToIconString());
    }

    private void DrawSteamInstructions() { }
}
