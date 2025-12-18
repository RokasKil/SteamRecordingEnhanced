using System;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.PluginServices;

public unsafe class KillService : AbstractService
{
    public delegate void AddActionLogMessageDelegate(uint logMessageRowId, BattleChara* source, BattleChara* target, uint unk4, uint unk5, uint unk6, uint unk7, uint unk8, uint unk9, float unk10);

    private Hook<AddActionLogMessageDelegate> addActionLogMessageHook = null!;

    public override void Init()
    {
        base.Init();
        addActionLogMessageHook = Hook<AddActionLogMessageDelegate>("E8 ?? ?? ?? ?? 41 8B CC 83 E9", AddActionLogMessageDetour);
        EnableHooks();
    }

    private void AddActionLogMessageDetour(uint logMessageRowId, BattleChara* source, BattleChara* target, uint unk4, uint unk5, uint unk6, uint unk7, uint unk8, uint unk9, float unk10)
    {
        addActionLogMessageHook.Original(logMessageRowId, source, target, unk4, unk5, unk6, unk7, unk8, unk9, unk10);
        Services.Log.Verbose($"AddActionLogMessageDetour {logMessageRowId} {(IntPtr)source:X}({GetName((GameObject*)source)}) {(IntPtr)target:X}({GetName((GameObject*)target)}) {unk4:X} {unk5:X} {unk6:X} {unk7:X} {unk8:X} {unk9:X} {unk10}");
        if (logMessageRowId is 557 or 558 or 559)
        {
            Services.Log.Debug($"AddActionLogMessageDetour {logMessageRowId} {(IntPtr)source:X}({GetName((GameObject*)source)}) {(IntPtr)target:X}({GetName((GameObject*)target)}) {unk4:X} {unk5:X} {unk6:X} {unk7:X} {unk8:X} {unk9:X} {unk10}");
            if (source != null)
            {
                var owner = GameObjectManager.Instance()->Objects.GetObjectByEntityId(source->OwnerId);
                Services.Log.Debug($"{source->ObjectKind} {source->OwnerId:X} {GetName(owner)}");
            }
        }
    }

    private string GetName(GameObject* chara)
    {
        if (chara != null)
        {
            return chara->NameString;
        }

        return "<null character>";
    }
}
