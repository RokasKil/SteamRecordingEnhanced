using Dalamud.Game.Chat;
using Lumina.Excel.Sheets;
using SteamRecordingEnhanced.PluginServices.Event.Metadata;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.PluginServices.Event;

public class QuestCompleteEvent : AbstractEvent
{
    private const uint QuestCompleteLogMessageId = 1602;

    public QuestCompleteEvent()
    {
        RegisterLogMessageEvent(OnQuestCompleteMessage, QuestCompleteLogMessageId);
    }

    private void OnQuestCompleteMessage(ILogMessage message)
    {
        if (!message.TryGetIntParameterWithLog(0, out var questId)) return;
        var questName = $"UNKNOWN_QUEST_{questId}";
        if (Services.DataManager.GetExcelSheet<Quest>().TryGetRow((uint)questId, out var questRow))
        {
            questName = questRow.Name.ToString();
        }

        Services.TimelineService.AddEvent("Quest completed", questName, GameEvent.QuestComplete);
        Services.Log.Info($"Quest completed: {message.FormatLogMessageForDebugging()}");
    }
}