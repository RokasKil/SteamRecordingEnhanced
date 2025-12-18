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
        Services.TimelineService.AddEvent("Duty started", Utils.GetTerritoryName(territoryTypeId), Services.Configuration.DutyStartedIcon, EventPriorities.DUTY_STARTED_PRIORITY);
    }

    private void DutyCompleted(object? sender, ushort territoryTypeId)
    {
        Services.TimelineService.AddEvent("Duty completed", Utils.GetTerritoryName(territoryTypeId), Services.Configuration.DutyCompleteIcon, EventPriorities.DUTY_COMPLETE_PRIORITY);
    }

    private void DutyWiped(object? sender, ushort territoryTypeId)
    {
        Services.TimelineService.AddEvent("Duty wiped", "", Services.Configuration.DutyWipedIcon, EventPriorities.DUTY_WIPED_PRIORITY);
    }

    public override void Dispose()
    {
        base.Dispose();
        Services.DutyState.DutyStarted -= DutyStarted;
        Services.DutyState.DutyWiped -= DutyWiped;
        Services.DutyState.DutyCompleted -= DutyCompleted;
    }
}
