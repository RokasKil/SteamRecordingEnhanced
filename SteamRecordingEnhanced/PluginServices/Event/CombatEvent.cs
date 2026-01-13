using System;
using Dalamud.Game.ClientState.Conditions;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.PluginServices.Event;

public class CombatEvent : AbstractEvent
{
    private const float MinimumEventDuration = 2f;

    private ulong? combatEventHandle;
    private DateTime combatEventStart;

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

        combatEventHandle = Services.TimelineService.StartEvent("Combat", "", "steam_combat");
        combatEventStart = DateTime.Now;
    }


    private void StopCombatEvent()
    {
        if (combatEventHandle.HasValue)
        {
            // in a rare scenario where the event ends right after it starts steam will convert it into an instantaneous event
            // this happened once in pvp and I couldn't replicate it in pve content
            // here we set a minimum event duration which also helps with displaying sub second combat events 
            var offset = MinimumEventDuration - MathF.Min((float)(DateTime.Now - combatEventStart).TotalSeconds, MinimumEventDuration);
            Services.TimelineService.EndEvent(combatEventHandle.Value, offset);
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