using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using SteamRecordingEnhanced.PluginServices.Event;
using SteamRecordingEnhanced.Utility;
using SteamRecordingEnhanced.Windows;

namespace SteamRecordingEnhanced;

public sealed class Plugin : IDalamudPlugin
{
    private const string ConfigCommandName = "/steamrecording";
    private const string EventCommandName = "/steamrecordingevent";

    public readonly WindowSystem WindowSystem = new("SteamRecordingEnhanced");

    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        Services.ConstructServices(pluginInterface, this);

        Services.Framework.RunOnFrameworkThread(Services.InitServices).ConfigureAwait(false).GetAwaiter().GetResult();
        ConfigWindow = new ConfigWindow();
        MainWindow = new MainWindow();

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        Services.CommandManager.AddHandler(ConfigCommandName, new CommandInfo(OnConfigCommand)
        {
            HelpMessage = "Open the configuration window"
        });
        Services.CommandManager.AddHandler(EventCommandName, new CommandInfo(OnEventCommand)
        {
            HelpMessage = "Manually place an event on the recording timeline. /steamrecordingevent [event title]"
        });

        Services.PluginInterface.UiBuilder.Draw += WindowSystem.Draw;

        Services.PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUi;

#if DEBUG
        Services.PluginInterface.UiBuilder.OpenMainUi += ToggleMainUi;
#endif
    }

    public void Dispose()
    {
        // Unregister all actions to not leak anything during disposal of plugin
        Services.PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
        Services.PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUi;
        Services.PluginInterface.UiBuilder.OpenMainUi -= ToggleMainUi;

        WindowSystem.RemoveAllWindows();
        Services.CommandManager.RemoveHandler(ConfigCommandName);
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

    public void ToggleMainUi()
    {
        MainWindow.Toggle();
    }
}
