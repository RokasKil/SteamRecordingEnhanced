using Dalamud.Game.ClientState.Conditions;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.PluginServices.Event;

public class CombatEvent : AbstractEvent
{
    private ulong? combatEventHandle;

    public CombatEvent()
    {
        Services.Condition.ConditionChange += ConditionChange;
        if (Services.ClientState.IsLoggedIn)
        {
            if (Services.Condition[ConditionFlag.InCombat])
            {
                StartCombatEvent();
            }
        }
    }

    private void ConditionChange(ConditionFlag flag, bool value)
    {
        if (flag == ConditionFlag.InCombat)
        {
            if (value)
            {
                StartCombatEvent();
            }
            else
            {
                StopCombatEvent();
            }
        }
    }

    private void StartCombatEvent()
    {
        StopCombatEvent();
        if (!Services.Configuration.HighlightCombat)
        {
            return;
        }

        combatEventHandle = Services.TimelineService.StartEvent("Combat", "Combat description", "steam_combat");
    }


    private void StopCombatEvent()
    {
        if (combatEventHandle.HasValue)
        {
            Services.TimelineService.EndEvent(combatEventHandle.Value);
            combatEventHandle = null;
        }
    }

    public override void Dispose()
    {
        Services.Condition.ConditionChange -= ConditionChange;
        StopCombatEvent();
        base.Dispose();
    }
}
