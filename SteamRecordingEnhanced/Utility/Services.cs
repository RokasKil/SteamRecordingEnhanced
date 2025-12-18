using System.Collections.Generic;
using Dalamud.Game;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using SteamRecordingEnhanced.PluginServices;

namespace SteamRecordingEnhanced.Utility;

public class Services
{
    private static readonly List<AbstractService> ServiceList = [];

    private static bool Disposed;
    [PluginService]
    public static IClientState ClientState { get; set; } = null!;
    // [PluginService]
    // public static ISigScanner SigScanner { get; set; } = null!;
    [PluginService]
    public static IDataManager DataManager { get; set; } = null!;
    [PluginService]
    public static ICondition Condition { get; set; } = null!;
    [PluginService]
    public static IGameGui GameGui { get; set; } = null!;
    [PluginService]
    public static IPluginLog Log { get; set; } = null!;
    [PluginService]
    public static IGameInteropProvider GameInteropProvider { get; set; } = null!;
    [PluginService]
    public static IFramework Framework { get; set; } = null!;
    [PluginService]
    public static ICommandManager CommandManager { get; set; } = null!;
    [PluginService]
    public static IAddonLifecycle AddonLifecycle { get; set; } = null!;
    [PluginService]
    public static ITextureProvider TextureProvider { get; set; } = null!;
    [PluginService]
    public static ITitleScreenMenu TitleScreenMenu { get; set; } = null!;
    [PluginService]
    public static IKeyState KeyState { get; set; } = null!;
    [PluginService]
    public static INotificationManager NotificationManager { get; set; } = null!;
    [PluginService]
    public static IChatGui ChatGui { get; set; } = null!;
    [PluginService]
    public static IObjectTable ObjectTable { get; set; } = null!;
    [PluginService]
    public static IPlayerState PlayerState { get; set; } = null!;
    [PluginService]
    public static IDutyState DutyState { get; set; } = null!;
    [PluginService]
    public static IPartyList PartyList { get; set; } = null!;
    public static IDalamudPluginInterface PluginInterface { get; set; } = null!;
    public static Configuration Configuration { get; set; } = null!;
    public static SteamService SteamService { get; set; } = null!;
    public static EventService EventService { get; set; } = null!;
    public static TimelineService TimelineService { get; set; } = null!;
    public static KillService KillService { get; set; } = null!;
    public static Plugin Plugin { get; set; } = null!;

    public static void ConstructServices(IDalamudPluginInterface pluginInterface, Plugin plugin)
    {
        pluginInterface.Create<Services>();
        Plugin = plugin;
        PluginInterface = pluginInterface;
        try
        {
            ServiceList.Add(SteamService = new());
            ServiceList.Add(EventService = new());
            ServiceList.Add(TimelineService = new());
            ServiceList.Add(KillService = new());
        }
        catch
        {
            Dispose();
            throw;
        }

        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
    }

    public static void InitServices()
    {
        try
        {
            ServiceList.ForEach(service => service.Init());
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    public static void Dispose()
    {
        if (Disposed) return;

        Disposed = true;
        ServiceList.ForEach(service => service.Dispose());
    }
}
