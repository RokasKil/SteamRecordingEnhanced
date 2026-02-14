using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.ImGuiNotification;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.Windows.Tabs;

public class StatusTab : ITab
{
    private const int FfxivDemoSteamAppId = 312060;
    private const string LaunchOptionsTemplate = @"""\path\to\XIVLauncher.exe""  %command%";

    public string Title => "Status";

    private string? launchOptionsLine;

    public StatusTab()
    {
        CheckXivLauncherLocation();
    }

    public void Draw()
    {
        using var textWrapPos = ImRaii.TextWrapPos(ImGui.GetFontSize() * 28);
        if (Util.IsWine() && ImGui.CollapsingHeader("Hey, wine enjoyer!"))
        {
            ImGui.TextWrapped("Currently this plugin is not supported on wine." +
                              " I'm hoping to get it sorted out in the future, but if you do manage to get it running," +
                              " please share some feedback on what you did and what OS you are running!");
        }

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
            ImGui.TextWrapped("Everything seems to be working.");
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
                return;
            }
        }

        ImGui.TextWrapped("Make sure the recording is running, I can't check that for you.");
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

    private void DrawSteamInstructions()
    {
        if (ImGui.CollapsingHeader("How to get this working on a non-steam copy of the game?"))
        {
            ImGui.TextWrapped("To enable steam recording functionality you need to launch FFXIV as an actual steam game" +
                              " (adding it as a non-steam game to steam will not work).");
            ImGui.TextWrapped("That can be achieved by following these instructions: ");
            using (GuiUtils.Bullet())
            {
                ImGui.TextWrapped("Get and download FINAL FANTASY XIV Online Free Trial from the steam store," +
                                  " you only need to download the launcher, no need to login or patch the game.");

                if (ImGui.Button("Take me to the steam store page"))
                {
                    OpenSteamLink($"steam://store/{FfxivDemoSteamAppId}");
                }
            }

            using (GuiUtils.Bullet())
            {
                ImGui.TextWrapped("Once the steam download finishes and you can click play in your library," +
                                  " right click the game in your library and open Properties or just click the button below.");
                if (ImGui.Button("Open Properties"))
                {
                    OpenSteamLink($"steam://gameproperties/{FfxivDemoSteamAppId}");
                }
            }

            using (GuiUtils.Bullet())
            {
                ImGui.TextWrapped("Go to 'General' tab and under 'LAUNCH OPTIONS' you'll enter a line that will make" +
                                  " steam launch XIVLauncher.exe instead of the game.");
                if (launchOptionsLine != null)
                {
                    ImGui.TextWrapped("Your XIVLauncher install location has been detected, the parameter has been" +
                                      " crafted for you, just copy and paste the line below.");
                    using (ImRaii.PushFont(UiBuilder.MonoFont))
                        ImGui.TextWrapped(launchOptionsLine);
                    if (ImGui.Button("Copy Launch Options to clipboard"))
                    {
                        ImGui.SetClipboardText(launchOptionsLine);
                    }
                }
                else
                {
                    ImGui.TextWrapped("Failed to detect your XIVLauncher install location, you'll have to edit this" +
                                      " template by replacing the \\path\\to\\XIVLauncher.exe part with full path to" +
                                      " your XIVLauncher.exe you usually start the game with.");
                    using (ImRaii.PushFont(UiBuilder.MonoFont))
                        ImGui.TextWrapped(LaunchOptionsTemplate);
                    if (ImGui.Button("Copy template to clipboard"))
                    {
                        ImGui.SetClipboardText(LaunchOptionsTemplate);
                    }
                }
            }

            using (GuiUtils.Bullet())
                ImGui.TextWrapped("Start the game and if you did everything correctly steam will show that you're playing" +
                                  " FINAL FANTASY XIV Online Free Trial but will actually start XIVLauncher.exe which" +
                                  " will just run your regular FFXIV install.");
            ImGui.TextWrapped("Now when you start the game through steam everything should just work, if you ran into" +
                              " any problems during this process you can contact the author via the Feedback button" +
                              " or Dalamud's discord.");
        }
    }

    private void CheckXivLauncherLocation()
    {
        try
        {
            string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (!string.IsNullOrEmpty(localAppDataPath))
            {
                string fullPath = Path.Combine(localAppDataPath, "XIVLauncher", "current", "XIVLauncher.exe");
                if (File.Exists(fullPath))
                {
                    launchOptionsLine = $"\"{fullPath}\" %command%";
                }
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private void OpenSteamLink(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo() { FileName = url, UseShellExecute = true });
        }
        catch (Exception e)
        {
            Services.Log.Error(e, $"Failed to open steam url: {url}");
            Services.NotificationManager.AddNotification(new()
            {
                Content = "Failed to open steam",
                Type = NotificationType.Error
            });
        }
    }
}