using System.Collections.Generic;
using SteamRecordingEnhanced.PluginServices.Event;

namespace SteamRecordingEnhanced.PluginServices;

public class EventService : AbstractService
{
    private readonly List<AbstractEvent> events = [];

    public override void Init()
    {
        events.Add(new PvpKillEvent());
        events.Add(new CombatEvent());
        events.Add(new AchievementUnlockEvent());
        events.Add(new DeathEvent());
        events.Add(new GameStateEvent());
        events.Add(new QuestCompleteEvent());
        events.Add(new TerritoryChangeEvent());
        events.Add(new DutyEvent());
        events.Add(new LevelUpEvent());
        events.Add(new FateEvent());
    }

    public override void Dispose()
    {
        base.Dispose();
        events.ForEach(eventObject => eventObject.Dispose());
    }
}