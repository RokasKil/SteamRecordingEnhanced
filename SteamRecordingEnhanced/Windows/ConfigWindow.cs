using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using SteamRecordingEnhanced.Windows.Tabs;

namespace SteamRecordingEnhanced.Windows;

public class ConfigWindow : Window
{
    private readonly ITab[] tabs =
    {
        new SettingsTab(),
        new StatusTab()
    };

    public ConfigWindow() : base(
        "Steam Recording Enhanced Configuration", ImGuiWindowFlags.AlwaysAutoResize)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, 1000)
        };
    }

    public override void Draw()
    {
        using var id = ImRaii.PushId(WindowName);
        using var tabBar = ImRaii.TabBar("");
        if (tabBar)
        {
            foreach (var tab in tabs)
            {
                using var tabId = ImRaii.PushId(tab.Title);
                using var tabItem = ImRaii.TabItem(tab.Title);
                if (tabItem)
                {
                    tab.Draw();
                }
            }
        }
    }
}
