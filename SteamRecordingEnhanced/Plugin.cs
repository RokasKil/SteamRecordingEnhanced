using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using SteamRecordingEnhanced.PluginServices.Event;
using SteamRecordingEnhanced.Utility;
using SteamRecordingEnhanced.Windows;

namespace SteamRecordingEnhanced;

public sealed class Plugin : IDalamudPlugin
{
    private const string ConfigCommandName = "/steamrec";
    private const string EventCommandName = "/steamrecevent";

    public readonly WindowSystem WindowSystem = new("SteamRecordingEnhanced");

    private ConfigWindow ConfigWindow { get; init; }
    private DebugWindow DebugWindow { get; init; }

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        Services.ConstructServices(pluginInterface, this);

        // init everything on framework thread
        Services.Framework.RunOnFrameworkThread(Services.InitServices).ConfigureAwait(false).GetAwaiter().GetResult();
        ConfigWindow = new ConfigWindow();
        DebugWindow = new DebugWindow();

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(DebugWindow);

        Services.CommandManager.AddHandler(ConfigCommandName, new CommandInfo(OnConfigCommand)
        {
            HelpMessage = "Open the configuration window"
        });
        Services.CommandManager.AddHandler(EventCommandName, new CommandInfo(OnEventCommand)
        {
            HelpMessage = "Manually place an event on the recording timeline. /steamrecevent [event title]"
        });

        Services.PluginInterface.UiBuilder.Draw += WindowSystem.Draw;
        Services.PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUi;
#if DEBUG
        Services.PluginInterface.UiBuilder.OpenMainUi += ToggleDebugUi;
#endif
    }

    public void Dispose()
    {
        Services.PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
        Services.PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUi;
        Services.PluginInterface.UiBuilder.OpenMainUi -= ToggleDebugUi;

        WindowSystem.RemoveAllWindows();
        Services.CommandManager.RemoveHandler(ConfigCommandName);
        Services.CommandManager.RemoveHandler(EventCommandName);
        Services.Dispose();
    }

    private void OnConfigCommand(string command, string args)
    {
        ToggleConfigUi();
    }

    private void OnEventCommand(string command, string args)
    {
        Services.TimelineService.AddEvent(args, "", "steam_marker", EventPriorities.USER_EVENT_PRIORITY);
    }

    public void ToggleConfigUi()
    {
        ConfigWindow.Toggle();
    }

    public void ToggleDebugUi()
    {
        DebugWindow.Toggle();
    }
}
