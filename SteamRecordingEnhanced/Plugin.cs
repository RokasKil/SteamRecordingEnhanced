using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using SteamRecordingEnhanced.Utility;
using SteamRecordingEnhanced.Windows;

namespace SteamRecordingEnhanced;

public sealed class Plugin : IDalamudPlugin
{
    private const string CommandName = "/steamrecording";

    public readonly WindowSystem WindowSystem = new("SteamRecordingEnhanced");

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        Services.ConstructServices(pluginInterface, this);

        Services.Framework.RunOnFrameworkThread(Services.InitServices).ConfigureAwait(false).GetAwaiter().GetResult();
        ConfigWindow = new ConfigWindow();
        MainWindow = new MainWindow();

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        Services.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "steamrecording yup"
        });

        Services.PluginInterface.UiBuilder.Draw += WindowSystem.Draw;

        Services.PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUi;

        Services.PluginInterface.UiBuilder.OpenMainUi += ToggleMainUi;
    }

    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    public void Dispose()
    {
        // Unregister all actions to not leak anything during disposal of plugin
        Services.PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
        Services.PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUi;
        Services.PluginInterface.UiBuilder.OpenMainUi -= ToggleMainUi;

        WindowSystem.RemoveAllWindows();
        Services.CommandManager.RemoveHandler(CommandName);
        Services.Dispose();
    }

    private void OnCommand(string command, string args)
    {
        MainWindow.Toggle();
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
