namespace SteamRecordingEnhanced.PluginServices.Event.Metadata;

// Higher the enum index higher the priority
public enum GameEvent
{
    [GameEventMetadata("steam_transfer", "Territory changed")]
    TerritoryChanged,
    [GameEventMetadata("steam_death", "Party member died")]
    PartyMemberDied,
    [GameEventMetadata("steam_death", "Player died")]
    PlayerDied,
    [GameEventMetadata("steam_chest", "Duty completed")]
    DutyComplete,
    [GameEventMetadata("steam_x", "Duty started")]
    DutyWiped,
    [GameEventMetadata("steam_attack", "Duty wiped")]
    DutyStarted,
    [GameEventMetadata("steam_effect", "Level up",
        "Works with Eureka and Occult Crescent leveling systems.")]
    LevelUp,
    [GameEventMetadata("steam_chest", "Fate completed",
        "Works with Eureka, Bozja and Occult Crescent fates and critical engagements.")]
    FateComplete,
    [GameEventMetadata("steam_ribbon", "Quest completed")]
    QuestComplete,
    [GameEventMetadata("steam_combat", "PVP kill",
        "Works by reading the combat log which is known to not be 100% accurate but it will works most of the time.")]
    PvpKill,
    [GameEventMetadata("steam_achievement", "Achievement unlocked")]
    AchievementUnlocked
}