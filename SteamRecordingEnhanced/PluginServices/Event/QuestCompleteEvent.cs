using System;
using Dalamud.Hooking;
using Lumina.Excel.Sheets;
using SteamRecordingEnhanced.PluginServices.Event.Metadata;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.PluginServices.Event;

public unsafe class QuestCompleteEvent : AbstractEvent
{
    private const uint QuestCompleteLogMessageRowId = 1602;

    private delegate void EventFrameworkAddLogDelegate(IntPtr eventFrameworkPtr, uint logMessageRowId, uint* logData, byte logDataLength);

    private readonly Hook<EventFrameworkAddLogDelegate> eventFrameworkAddLogHook;

    public QuestCompleteEvent()
    {
        //Client::Game::Event::EventHandler.vf45
        eventFrameworkAddLogHook = Hook<EventFrameworkAddLogDelegate>("E9 ?? ?? ?? ?? 45 84 C9 74 ?? 41 8B 10", EventFrameworkAddLogDetour);
        EnableHooks();
    }

    private void EventFrameworkAddLogDetour(IntPtr eventFrameworkPtr, uint logMessageRowId, uint* logData, byte logDataLength)
    {
        if (logMessageRowId == QuestCompleteLogMessageRowId && logDataLength > 0)
        {
            var questId = logData[0];
            var questName = $"UNKNOWN_QUEST_{questId}";
            if (Services.DataManager.GetExcelSheet<Quest>().TryGetRow(questId, out var questRow))
            {
                questName = questRow.Name.ToString();
            }

            Services.TimelineService.AddEvent("Quest completed", questName, GameEvent.QuestComplete);
        }

        eventFrameworkAddLogHook.Original(eventFrameworkPtr, logMessageRowId, logData, logDataLength);
    }
}