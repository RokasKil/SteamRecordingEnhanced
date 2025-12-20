using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using SteamRecordingEnhanced.Steam;

namespace SteamRecordingEnhanced.Utility;

public static class GuiUtils
{
    // Used for scaling with non standard font sizes
    public const float ICON_SIZE = 24f;
    public const float BASE_FONT_SIZE = 17f;

    public static float IconRatio => ImGui.GetFontSize() / BASE_FONT_SIZE;

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

    // ImGui alchemy ahead
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

        IDalamudTextureWrap? selectedIcon = null;
        if (Enum.TryParse(displayValue, out SteamIcon icon))
        {
            selectedIcon = GetIconTextureWrap(icon);
        }

        var preComboPos = ImGui.GetCursorPos();
        using (ImRaii.ItemWidth(ImGui.CalcTextSize("Number").X + ImGui.GetStyle().FramePadding.X * 2 + ImGui.GetFrameHeight()))
        {
            using var combo = ImRaii.Combo("##combo", selectedIcon != null ? "" : displayValue);
            if (selectedIcon != null && ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(displayValue);
            }

            if (combo)
            {
                if (ImGui.Selectable("None", displayValue == "None"))
                {
                    value = "";
                    return true;
                }

                foreach (SteamIcon enumValue in Enum.GetValues(typeof(SteamIcon)))
                {
                    var iconName = $"steam_{enumValue.ToString().ToLower()}";
                    using var buttonId = ImRaii.PushId(iconName);
                    IDalamudTextureWrap? iconImage = GetIconTextureWrap(enumValue);

                    var cursorPos = ImGui.GetCursorPos();
                    using (PushFont(Services.IconService.IconFont, iconImage != null))
                        if (ImGui.Selectable(iconImage == null ? enumValue.ToString() : "", enumValue.ToString() == displayValue))
                        {
                            value = enumValue == SteamIcon.Number ? $"steam_{number}" : iconName;

                            changed = true;
                        }

                    if (iconImage != null)
                    {
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip(enumValue.ToString());
                        }

                        ImGui.SetCursorPos(cursorPos);
                        ImGui.Image(iconImage.Handle, new Vector2(ICON_SIZE, ICON_SIZE) * IconRatio);
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
        var finalPos = ImGui.GetCursorPos();
        if (selectedIcon != null)
        {
            ImGui.SetCursorPos(preComboPos + ImGui.GetStyle().FramePadding);
            ImGui.Image(selectedIcon.Handle, new Vector2(17, 17) * IconRatio);
            ImGui.SetCursorPos(finalPos);
        }

        return changed;
    }

    private static IDalamudTextureWrap? GetIconTextureWrap(SteamIcon icon)
    {
        var iconName = $"steam_{icon.ToString().ToLower()}";
        var iconPath = Services.IconService.GetIconPath(iconName, icon.GetIconUrl());

        if (iconPath != null)
        {
            return Services.TextureProvider.GetFromFileAbsolute(iconPath).GetWrapOrDefault();
        }

        return null;
    }

    public static ImRaii.IEndObject Bullet()
    {
        ImGui.Bullet();
        var indent = ImRaii.PushIndent(ImGui.GetTreeNodeToLabelSpacing());
        return new ImRaii.EndUnconditionally(() => indent.Dispose(), true);
    }

    public static ImRaii.IEndObject PushFont(IFontHandle font, bool condition = true)
    {
        if (condition)
        {
            var disposable = font.Push();
            return new ImRaii.EndConditionally(() => disposable.Dispose(), true);
        }

        return ImRaii.IEndObject.Empty;
    }
}
