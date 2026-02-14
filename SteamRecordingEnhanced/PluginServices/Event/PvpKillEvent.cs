using System;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Lumina.Excel.Sheets;
using SteamRecordingEnhanced.PluginServices.Event.Metadata;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.PluginServices.Event;

public unsafe class PvpKillEvent : AbstractEvent
{
    private const uint DeathLogMessageRowId = 557;
    private const uint WolvesDenTerritoryId = 250;
    // One is probably for ranked other is for normal games
    private const uint CrystallineConflictTerritoryIntendedUseId1 = 28;
    private const uint CrystallineConflictTerritoryIntendedUseId2 = 37;

    private delegate void AddActionLogMessageDelegate(uint logMessageRowId, Character* source, Character* target, uint unk4, uint unk5, uint unk6, uint unk7, uint unk8, uint unk9, float unk10);

    private readonly Hook<AddActionLogMessageDelegate> addActionLogMessageHook;

    public PvpKillEvent()
    {
        addActionLogMessageHook = Hook<AddActionLogMessageDelegate>("E8 ?? ?? ?? ?? 41 8B CC 83 E9", AddActionLogMessageDetour);
        EnableHooks();
    }

    private void AddActionLogMessageDetour(uint logMessageRowId, Character* source, Character* target, uint unk4, uint unk5, uint unk6, uint unk7, uint unk8, uint unk9, float unk10)
    {
        if (Services.ClientState.IsPvP &&
            logMessageRowId == DeathLogMessageRowId &&
            source != null &&
            target != null &&
            target->ObjectKind == ObjectKind.Pc &&
            Services.ObjectTable.LocalPlayer != null &&
            (
                (IntPtr)source == Services.ObjectTable.LocalPlayer.Address ||
                source->OwnerId == Services.ObjectTable.LocalPlayer.EntityId
            )
           )
        {
            Services.TimelineService.AddEvent("Player killed", $"You killed {GetName(target)}!", GameEvent.PvpKill, -2);
        }

        addActionLogMessageHook.Original(logMessageRowId, source, target, unk4, unk5, unk6, unk7, unk8, unk9, unk10);
    }

    private string GetName(Character* character)
    {
        if (ShouldShowName())
        {
            return $"{character->NameString} ({Utils.GetJobAbbreviation(character->ClassJob)})";
        }
        else
        {
            return Utils.GetJobName(character->ClassJob);
        }
    }

    // Only show the name in the den and CC games
    public static bool ShouldShowName()
    {
        return Services.ClientState.TerritoryType == WolvesDenTerritoryId ||
               Services.DataManager.GetExcelSheet<TerritoryType>().GetRowOrDefault(Services.ClientState.TerritoryType)?.TerritoryIntendedUse.RowId is
                   CrystallineConflictTerritoryIntendedUseId1 or CrystallineConflictTerritoryIntendedUseId2;
    }
}