namespace SteamRecordingEnhanced.PluginServices.Event;

public static class EventPriorities
{
    public const uint USER_EVENT_PRIORITY = 1000;
    public const uint ACHIEVEMENT_UNLOCKED_PRIORITY = 11;
    public const uint PVP_KILL_PRIORITY = 10;
    public const uint QUEST_COMPLETE_PRIORITY = 9;
    public const uint LEVEL_UP_PRIORITY = 8;
    public const uint DUTY_STARTED_PRIORITY = 5;
    public const uint DUTY_WIPED_PRIORITY = 5;
    public const uint DUTY_COMPLETE_PRIORITY = 5;
    public const uint PLAYER_DIED_PRIORITY = 4;
    public const uint PARTY_MEMBER_DIED_PRIORITY = 3;
    public const uint TERRITORY_CHANGED_PRIORITY = 0;
}
