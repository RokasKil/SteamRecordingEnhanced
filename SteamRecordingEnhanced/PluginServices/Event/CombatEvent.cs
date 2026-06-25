using System;
using Dalamud.Game.ClientState.Conditions;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.PluginServices.Event;

public class CombatEvent : AbstractEvent
{
    private const float MinimumEventDuration = 2f;

    private DateTime? combatEventStart;

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

        combatEventStart = DateTime.Now;
    }


    private void StopCombatEvent()
    {
        if (!combatEventStart.HasValue) return;
        // I set a minimum event duration which helps to display sub second combat events for better UX
        var startOffset = (float)(combatEventStart.Value - DateTime.Now).TotalSeconds;
        var duration = MathF.Max(-startOffset, MinimumEventDuration);
        Services.TimelineService.AddRangeEvent("Combat", "", "steam_combat", duration, startOffset);
        combatEventStart = null;
    }

    public override void Dispose()
    {
        Services.Condition.ConditionChange -= ConditionChange;
        StopCombatEvent();
        base.Dispose();
    }
}