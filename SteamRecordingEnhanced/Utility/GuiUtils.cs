using System;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using SteamRecordingEnhanced.Steam;

namespace SteamRecordingEnhanced.Utility;

public class GuiUtils
{
    public static bool Combo<T>(string title, ref T value, ImGuiComboFlags flags = ImGuiComboFlags.None) where T : Enum
    {
        using var id = ImRaii.PushId(title);
        using var combo = ImRaii.Combo(title, value.ToString(), flags);
        if (combo)
        {
            foreach (T enumValue in Enum.GetValues(typeof(T)))
            {
                if (ImGui.Selectable(enumValue.ToString(), value.Equals(enumValue)))
                {
                    value = enumValue;
                    return true;
                }
            }
        }

        return false;
    }

    public static bool SteamIconSelect(string title, ref string value)
    {
        using var id = ImRaii.PushId(title);
        bool changed = false;
        string displayValue = value.StartsWith("steam_") ? value["steam_".Length..] : value;
        if (displayValue.Length > 0)
        {
            displayValue = string.Concat(displayValue[0].ToString().ToUpper(), displayValue.AsSpan(1));
        }

        if (displayValue == "")
        {
            displayValue = "None";
        }

        bool isNumber = int.TryParse(displayValue, out var number);
        if (isNumber)
        {
            displayValue = "Number";
        }
        else
        {
            number = 1;
        }

        using (ImRaii.ItemWidth(ImGui.GetFontSize() * 8))
        {
            using var combo = ImRaii.Combo("##combo", displayValue);
            if (combo)
            {
                if (ImGui.Selectable("None", displayValue == "None"))
                {
                    value = "";
                    return true;
                }

                foreach (SteamIcon enumValue in Enum.GetValues(typeof(SteamIcon)))
                {
                    if (ImGui.Selectable(enumValue.ToString(), enumValue.ToString() == displayValue))
                    {
                        value = enumValue == SteamIcon.Number ? $"steam_{number}" : $"steam_{enumValue.ToString().ToLower()}";

                        changed = true;
                    }
                }
            }
        }

        if (isNumber)
        {
            using var width = ImRaii.ItemWidth(ImGui.GetFontSize() * 2);
            ImGui.SameLine();
            if (ImGui.InputInt("##Number", ref number, flags: ImGuiInputTextFlags.CharsDecimal))
            {
                number = Math.Clamp(number, 1, 99);
                value = $"steam_{number}";
                changed = true;
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("A Number from 1 to 99 to be used as the icon");
            }
        }

        ImGui.SameLine();
        ImGui.TextUnformatted(title);

        return changed;
    }
}
