using Lumina.Excel;
using Lumina.Excel.Sheets;
using SteamRecordingEnhanced.PluginServices.Event.Metadata;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.PluginServices.Event;

public class AchievementUnlockEvent : AbstractEvent
{
    public AchievementUnlockEvent()
    {
        Services.UnlockState.Unlock += OnUnlock;
    }

    private void OnUnlock(RowRef rowRef)
    {
        if (!rowRef.TryGetValue<Achievement>(out var achievementRow)) return;
        var achievementName = achievementRow.Name.ToString();
        Services.TimelineService.AddEvent("Achievement unlocked", $"{achievementName}", GameEvent.AchievementUnlocked);
    }
}