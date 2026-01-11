using SteamRecordingEnhanced.PluginServices.Event.Metadata;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.PluginServices.Event;

public class DutyEvent : AbstractEvent
{
    public DutyEvent()
    {
        Services.DutyState.DutyStarted += DutyStarted;
        Services.DutyState.DutyWiped += DutyWiped;
        Services.DutyState.DutyCompleted += DutyCompleted;
    }

    private void DutyStarted(object? sender, ushort territoryTypeId)
    {
        Services.TimelineService.AddEvent("Duty started", Utils.GetContentOrTerritoryName(territoryTypeId), GameEvent.DutyStarted);
    }

    private void DutyCompleted(object? sender, ushort territoryTypeId)
    {
        Services.TimelineService.AddEvent("Duty completed", Utils.GetContentOrTerritoryName(territoryTypeId), GameEvent.DutyComplete);
    }

    private void DutyWiped(object? sender, ushort territoryTypeId)
    {
        Services.TimelineService.AddEvent("Duty wiped", Utils.GetContentOrTerritoryName(territoryTypeId), GameEvent.DutyWiped);
    }

    public override void Dispose()
    {
        base.Dispose();
        Services.DutyState.DutyStarted -= DutyStarted;
        Services.DutyState.DutyWiped -= DutyWiped;
        Services.DutyState.DutyCompleted -= DutyCompleted;
    }
}