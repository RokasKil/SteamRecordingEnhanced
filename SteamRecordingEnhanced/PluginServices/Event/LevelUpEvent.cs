using Dalamud.Hooking;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.PluginServices.Event;

public class LevelUpEvent : AbstractEvent
{
    private delegate void LevelUpDelegate(uint entityId, uint jobId, ushort level);

    private readonly Hook<LevelUpDelegate> levelUpHook;

    public LevelUpEvent()
    {
        levelUpHook = Hook<LevelUpDelegate>("40 55 57 41 55 41 56 41 57 48 83 EC ?? 44 8B F9", LevelUpDetour);
        EnableHooks();
    }

    private void LevelUpDetour(uint entityId, uint jobId, ushort level)
    {
        levelUpHook.Original(entityId, jobId, level);
        if (Services.ObjectTable.LocalPlayer?.EntityId == entityId)
        {
            Services.TimelineService.AddEvent("Level up", $"{Utils.GetJobName(jobId)} Lv. {level}", Services.Configuration.LevelUpIcon, EventPriorities.LEVEL_UP_PRIORITY);
        }
    }
}