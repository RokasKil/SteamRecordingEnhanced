using Dalamud.Game.Chat;
using SteamRecordingEnhanced.PluginServices.Event.Metadata;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.PluginServices.Event;

public class LevelUpEvent : AbstractEvent
{
    // Regular content
    private const uint LevelUpMessageId = 590;
    private const uint LevelUpOtherJobMessageId = 591;
    // Eureka
    private const uint ElementalLevelUpMessageId = 9053;
    // Bozja
    private const uint ResistanceRankUpMessageId = 9630;
    //Crescent
    private const uint LevelUpKnowledgeMessageId = 10955;
    private const uint LevelUpPhantomJobMessageId = 10957;


    public LevelUpEvent()
    {
        RegisterLogMessageEvent(OnLevelUpMessage, LevelUpMessageId, LevelUpOtherJobMessageId);
        RegisterLogMessageEvent(OnElementalLevelUpMessage, ElementalLevelUpMessageId);
        RegisterLogMessageEvent(OnResistanceRankUpMessage, ResistanceRankUpMessageId);
        RegisterLogMessageEvent(OnKnowledgeLevelUpMessage, LevelUpKnowledgeMessageId);
        RegisterLogMessageEvent(OnPhantomJobLevelUpMessage, LevelUpPhantomJobMessageId);
    }

    private void OnLevelUpMessage(ILogMessage message)
    {
        if (!message.SourceEntity.IsLocalPlayer()) return;
        if (!message.TryGetIntParameterWithLog(0, out var jobIndex)) return;
        if (!message.TryGetIntParameterWithLog(1, out var lvl)) return;
        Services.TimelineService.AddEvent("Level up", $"{Utils.GetJobName((uint)jobIndex)} Lv. {lvl}", GameEvent.LevelUp);
    }

    private void OnElementalLevelUpMessage(ILogMessage message)
    {
        if (!message.SourceEntity.IsLocalPlayer()) return;
        if (!message.TryGetIntParameter(0, out var lvl)) return;
        Services.TimelineService.AddEvent("Elemental level up", $"Lv. {lvl}", GameEvent.LevelUp);
    }

    private void OnResistanceRankUpMessage(ILogMessage message)
    {
        if (!message.SourceEntity.IsLocalPlayer()) return;
        if (!message.TryGetIntParameter(0, out var lvl)) return;
        Services.TimelineService.AddEvent("Resistance rank up", $"Rank {lvl}", GameEvent.LevelUp);
    }

    private void OnKnowledgeLevelUpMessage(ILogMessage message)
    {
        if (!message.SourceEntity.IsLocalPlayer()) return;
        if (!message.TryGetIntParameter(0, out var lvl)) return;
        Services.TimelineService.AddEvent("Knowledge level up", $"Lv. {lvl}", GameEvent.LevelUp);
    }

    private void OnPhantomJobLevelUpMessage(ILogMessage message)
    {
        if (!message.SourceEntity.IsLocalPlayer()) return;
        if (!message.TryGetIntParameterWithLog(0, out var jobIndex)) return;
        if (!message.TryGetIntParameterWithLog(1, out var lvl)) return;
        Services.TimelineService.AddEvent("Phantom Job level up", $"{Utils.GetPhantomJobName((uint)jobIndex)} Lv. {lvl}", GameEvent.LevelUp);
    }
}