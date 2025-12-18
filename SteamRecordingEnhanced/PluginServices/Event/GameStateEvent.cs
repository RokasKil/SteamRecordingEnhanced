using Dalamud.Plugin.Services;
using SteamRecordingEnhanced.Steam;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.PluginServices.Event;

public class GameStateEvent : AbstractEvent
{
    public GameStateEvent()
    {
        Services.Framework.Update += Tick;
    }

    private void Tick(IFramework framework)
    {
        var gameMode = Services.ClientState.IsLoggedIn ? TimelineGameMode.Playing : TimelineGameMode.Menus;
        if (Services.GameGui.GetAddonByName("NowLoading").IsVisible)
        {
            gameMode = TimelineGameMode.LoadingScreen;
        }

        if (Services.TimelineService.CurrentGameMode != gameMode)
        {
            Services.TimelineService.SetGameMode(gameMode);
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        Services.Framework.Update -= Tick;
    }
}
