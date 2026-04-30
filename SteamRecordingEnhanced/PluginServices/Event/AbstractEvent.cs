using System.Collections.Generic;
using Dalamud.Plugin.Services;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.PluginServices.Event;

public abstract class AbstractEvent : HookOwner
{
    private readonly Dictionary<IChatGui.OnLogMessageDelegate, List<uint>> registeredLogMessages = new();

    protected void RegisterLogMessageEvent(IChatGui.OnLogMessageDelegate callback, params uint[] messageIds)
    {
        if (!registeredLogMessages.TryGetValue(callback, out var list))
        {
            list = registeredLogMessages[callback] = [];
        }

        list.AddRange(messageIds);
        Services.EventService.RegisterLogMessageEvent(callback, messageIds);
    }

    protected void UnregisterLogMessageEvent(IChatGui.OnLogMessageDelegate callback, params uint[] messageIds)
    {
        if (registeredLogMessages.TryGetValue(callback, out var list))
        {
            foreach (var messageId in messageIds)
            {
                list.Remove(messageId);
            }

            if (list.Count == 0)
            {
                registeredLogMessages.Remove(callback);
            }
        }

        Services.EventService.UnregisterLogMessageEvent(callback, messageIds);
    }

    public override void Dispose()
    {
        base.Dispose();
        foreach (var registeredLogMessage in registeredLogMessages)
        {
            UnregisterLogMessageEvent(registeredLogMessage.Key, registeredLogMessage.Value.ToArray());
        }
    }
}