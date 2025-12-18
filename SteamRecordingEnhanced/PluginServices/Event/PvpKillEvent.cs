using System;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.PluginServices.Event;

public unsafe class PvpKillEvent : AbstractEvent
{
    private const uint DeathLogMessageRowId = 557;

    public delegate void AddActionLogMessageDelegate(uint logMessageRowId, BattleChara* source, BattleChara* target, uint unk4, uint unk5, uint unk6, uint unk7, uint unk8, uint unk9, float unk10);

    private readonly Hook<AddActionLogMessageDelegate> addActionLogMessageHook;

    public PvpKillEvent()
    {
        addActionLogMessageHook = Hook<AddActionLogMessageDelegate>("E8 ?? ?? ?? ?? 41 8B CC 83 E9", AddActionLogMessageDetour);
        EnableHooks();
    }

    private void AddActionLogMessageDetour(uint logMessageRowId, BattleChara* source, BattleChara* target, uint unk4, uint unk5, uint unk6, uint unk7, uint unk8, uint unk9, float unk10)
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
            Services.TimelineService.AddEvent("Player killed", $"You killed {target->NameString}!", Services.Configuration.PvpKillIcon, EventPriorities.PVP_KILL_PRIORITY, -2);
        }

        addActionLogMessageHook.Original(logMessageRowId, source, target, unk4, unk5, unk6, unk7, unk8, unk9, unk10);
    }
}
