using Dalamud.Game.DutyState;
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

    private void DutyStarted(IDutyStateEventArgs args)
    {
        Services.TimelineService.AddEvent("Duty started", Utils.GetContentOrTerritoryName(args.TerritoryType.RowId), GameEvent.DutyStarted);
    }

    private void DutyCompleted(IDutyStateEventArgs args)
    {
        Services.TimelineService.AddEvent("Duty completed", Utils.GetContentOrTerritoryName(args.TerritoryType.RowId), GameEvent.DutyComplete);
    }

    private void DutyWiped(IDutyStateEventArgs args)
    {
        Services.TimelineService.AddEvent("Duty wiped", Utils.GetContentOrTerritoryName(args.TerritoryType.RowId), GameEvent.DutyWiped);
    }

    public override void Dispose()
    {
        base.Dispose();
        Services.DutyState.DutyStarted -= DutyStarted;
        Services.DutyState.DutyWiped -= DutyWiped;
        Services.DutyState.DutyCompleted -= DutyCompleted;
    }
}