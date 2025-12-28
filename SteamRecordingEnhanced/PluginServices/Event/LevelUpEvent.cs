using System;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.PluginServices.Event;

public unsafe class LevelUpEvent : AbstractEvent
{
    // Regular content
    private const uint LevelUpMessageId = 590;
    private const uint LevelUpOtherJobMessageId = 591;
    // Eureka
    private const uint ElementalLevelUpMessageId = 9053;
    // Bozja
    private const uint LevelUpResistanceRankMessageId = 9630;
    //Crescent
    private const uint LevelUpKnowledgeMessageId = 10955;
    private const uint LevelUpPhantomJobMessageId = 10957;

    private delegate void LevelLogMessageDelegate(uint messageId, Character* character, uint num1, uint num2, uint unk5, float unk6);

    private readonly Hook<LevelLogMessageDelegate> levelLogMessageHook;

    public LevelUpEvent()
    {
        levelLogMessageHook = Hook<LevelLogMessageDelegate>("E9 ?? ?? ?? ?? 45 88 83", LevelLogMessageDetour);
        EnableHooks();
    }

    private void LevelLogMessageDetour(uint messageId, Character* character, uint num1, uint num2, uint unk5, float unk6)
    {
        levelLogMessageHook.Original(messageId, character, num1, num2, unk5, unk6);
        if (character == null || (IntPtr)character != Services.ObjectTable.LocalPlayer?.Address)
        {
            return;
        }

        string title;
        string description;
        if (messageId is LevelUpMessageId or LevelUpOtherJobMessageId)
        {
            title = "Level up";
            description = $"{Utils.GetJobName(num1)} Lv. {num2}";
        }
        else if (messageId is ElementalLevelUpMessageId)
        {
            title = "Elemental level up";
            description = $"Lv. {num1}";
        }
        // TODO: Bozja rank up doesn't work (I think, hard to test when already maxed rank and the instances are kinda dead)
        else if (messageId is LevelUpResistanceRankMessageId)
        {
            title = "Resistance rank up";
            description = $"Rank {num2}";
        }
        else if (messageId is LevelUpKnowledgeMessageId)
        {
            title = "Knowledge level up";
            description = $"Lv. {num1}";
        }
        else if (messageId is LevelUpPhantomJobMessageId)
        {
            title = "Phantom Job level up";
            description = $"{Utils.GetPhantomJobName(num1)} Lv. {num2}";
        }
        else
        {
            return;
        }

        Services.TimelineService.AddEvent(title, description, Services.Configuration.LevelUpIcon, EventPriorities.LEVEL_UP_PRIORITY);
    }
}