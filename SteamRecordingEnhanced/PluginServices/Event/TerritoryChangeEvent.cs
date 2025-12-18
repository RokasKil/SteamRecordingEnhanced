using Dalamud.Game.ClientState.Conditions;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.PluginServices.Event;

public class TerritoryChangeEvent : AbstractEvent
{
    public TerritoryChangeEvent()
    {
        Services.ClientState.TerritoryChanged += TerritoryChanged;
        if (Services.ClientState.IsLoggedIn)
        {
            StartPhase();
        }
    }

    private void TerritoryChanged(ushort territoryTypeId)
    {
        Services.TimelineService.AddEvent("Territory changed", Utils.GetTerritoryName(territoryTypeId), Services.Configuration.TerritoryChangedIcon, EventPriorities.TERRITORY_CHANGED_PRIORITY);
        StartPhase();
    }

    private void StartPhase()
    {
        Services.TimelineService.EndGamePhase();
        for (int i = 0; i < Services.Condition.MaxEntries; i++)
        {
            if (Services.Condition[i])
            {
                Services.Log.Debug($"{(ConditionFlag)i}");
            }
        }

        if (Services.Configuration.SessionsOnlyInInstance && !Services.Condition.Any(
                ConditionFlag.BoundByDuty,
                ConditionFlag.BoundByDuty56,
                ConditionFlag.BoundByDuty95))
        {
            // Not in instance
            return;
        }

        string world = Services.PlayerState.HomeWorld.ValueNullable?.Name.ToString() ?? "UNKNOWN_WORLD";
        string name = $"{Services.PlayerState.CharacterName}@{world}";
        string territory = Utils.GetTerritoryName(Services.ClientState.TerritoryType);

        Services.TimelineService.StartGamePhase();
        // Non searchable info
        Services.TimelineService.SetGamePhaseAttribute("name", name, 1);
        Services.TimelineService.SetGamePhaseAttribute("territory", territory, 0);
        // Searchable info that doesn't display text if the icon is set for some reason?
        Services.TimelineService.AddGamePhaseTag(name + " " + territory, "steam_person", "search_tag");

        // Timeline tooltip
        Services.TimelineService.SetTimelineTooltip(territory);
    }

    public override void Dispose()
    {
        base.Dispose();
        Services.ClientState.TerritoryChanged -= TerritoryChanged;
    }
}
