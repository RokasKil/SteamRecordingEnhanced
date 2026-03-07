using System.Collections.Generic;
using Dalamud.Game.Chat;
using Dalamud.Plugin.Services;
using SteamRecordingEnhanced.PluginServices.Event;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.PluginServices;

public class EventService : AbstractService
{
    private readonly List<AbstractEvent> events = [];
    private readonly Dictionary<uint, List<IChatGui.OnLogMessageDelegate>> logMessageCallbacks = new();

    public override void Init()
    {
        events.Add(new PvpKillEvent());
        events.Add(new CombatEvent());
        events.Add(new AchievementUnlockEvent());
        events.Add(new DeathEvent());
        events.Add(new GameStateEvent());
        events.Add(new QuestCompleteEvent());
        events.Add(new TerritoryChangeEvent());
        events.Add(new DutyEvent());
        events.Add(new LevelUpEvent());
        events.Add(new FateEvent());
        Services.ChatGui.LogMessage += OnLogMessage;
    }

    public void RegisterLogMessageEvent(IChatGui.OnLogMessageDelegate callback, params uint[] messageIds)
    {
        foreach (var messageId in messageIds)
        {
            if (!logMessageCallbacks.ContainsKey(messageId))
            {
                logMessageCallbacks[messageId] = [];
            }

            logMessageCallbacks[messageId].Add(callback);
        }
    }

    public void UnregisterLogMessageEvent(IChatGui.OnLogMessageDelegate callback, params uint[] messageIds)
    {
        foreach (var messageId in messageIds)
        {
            if (!logMessageCallbacks.TryGetValue(messageId, out var callbacks)) continue;
            callbacks.Remove(callback);
            if (callbacks.Count == 0) logMessageCallbacks.Remove(messageId);
        }
    }

    private void OnLogMessage(ILogMessage message)
    {
        if (!logMessageCallbacks.TryGetValue(message.LogMessageId, out var callbacks)) return;
        foreach (var callback in callbacks)
        {
            callback(message);
        }

        Services.Log.Verbose(message.DebugFormatLog());
    }


    public override void Dispose()
    {
        base.Dispose();
        events.ForEach(eventObject => eventObject.Dispose());
        Services.ChatGui.LogMessage -= OnLogMessage;
    }
}