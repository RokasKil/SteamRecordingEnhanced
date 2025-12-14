using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;

namespace SteamRecordingEnhanced.Windows.Tabs;

internal class AboutTab : ITab
{
    public string Title => "About";

    public void Draw()
    {
        using var textWrapPos = ImRaii.TextWrapPos(ImGui.GetFontSize() * 28);
        ImGui.TextWrapped("Steam recording enhanced.");
    }

    private void WrappedBulletText(string text)
    {
        ImGui.Bullet();
        ImGui.TextWrapped(text);
    }
}
