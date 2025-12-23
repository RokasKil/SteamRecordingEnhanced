using System;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Component.Text;
using FFXIVClientStructs.STD;
using Lumina.Excel.Sheets;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.PluginServices.Event;

public unsafe class AchievementUnlockEvent : AbstractEvent
{
    private const uint AchievementLogMessageRowId = 952;

    private delegate void AddLogMessageDelegate(IntPtr raptureLogModule, uint logMessageRowId, Character* character, StdDeque<TextParameter>* logParameters);

    private readonly Hook<AddLogMessageDelegate> addLogMessageHook;

    public AchievementUnlockEvent()
    {
        addLogMessageHook = Hook<AddLogMessageDelegate>("E8 ?? ?? ?? ?? 49 3B FE 0F 85", AddLogMessageDetour);
        EnableHooks();
    }

    private void AddLogMessageDetour(IntPtr raptureLogModule, uint logMessageRowId, Character* character, StdDeque<TextParameter>* logParameters)
    {
        if (logMessageRowId == AchievementLogMessageRowId &&
            character != null &&
            Services.ObjectTable.LocalPlayer?.Address == (IntPtr)character &&
            logParameters != null &&
            logParameters->Count > 0 &&
            (*logParameters)[0].Type == TextParameterType.Integer
           )
        {
            var achievementId = (uint)(*logParameters)[0].IntValue;
            var achievementName = $"UNKNOWN_ACHIEVEMENT_{achievementId}";
            if (Services.DataManager.GetExcelSheet<Achievement>().TryGetRow(achievementId, out var achievementRow))
            {
                achievementName = achievementRow.Name.ToString();
            }

            Services.TimelineService.AddEvent("Achievement unlocked", $"{achievementName}", Services.Configuration.AchievementUnlockedIcon, EventPriorities.ACHIEVEMENT_UNLOCKED_PRIORITY);
        }

        addLogMessageHook.Original(raptureLogModule, logMessageRowId, character, logParameters);
    }
}