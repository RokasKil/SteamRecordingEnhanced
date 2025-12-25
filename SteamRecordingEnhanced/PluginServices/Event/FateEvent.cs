using System.Linq;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.NativeWrapper;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.PluginServices.Event;

public class FateEvent : AbstractEvent
{
    private static readonly int[] CriticalEngagementIcons = [63909, 63910, 63911, 63912];

    public FateEvent()
    {
        Services.AddonLifecylce.RegisterListener(AddonEvent.PreSetup, "FateReward", FateRewardPreSetup);
    }


    private void FateRewardPreSetup(AddonEvent type, AddonArgs args)
    {
        if (args is not AddonSetupArgs addonSetupArgs)
        {
            return;
        }

        var nameValue = addonSetupArgs.AtkValueEnumerable.FirstOrDefault();
        var iconValue = addonSetupArgs.AtkValueEnumerable.ElementAtOrDefault(1);
        var successValue = addonSetupArgs.AtkValueEnumerable.ElementAtOrDefault(2);
        if (nameValue.IsNull || nameValue.ValueType != AtkValueType.String ||
            iconValue.IsNull || iconValue.ValueType != AtkValueType.Int ||
            successValue.IsNull || successValue.ValueType != AtkValueType.Int ||
            (int)successValue.GetValue()! == 0)
        {
            return;
        }

        var iconId = (int)iconValue.GetValue()!;
        var title = CriticalEngagementIcons.Contains(iconId) ? "Critical Engagement completed" : "Fate completed";
        var description = $"{nameValue.GetValue()}";
        Services.TimelineService.AddEvent(title, description, Services.Configuration.FateCompleteIcon, EventPriorities.FATE_COMPLETE_PRIORITY);
    }

    public override void Dispose()
    {
        base.Dispose();
        Services.AddonLifecylce.UnregisterListener(AddonEvent.PreSetup, "FateReward", FateRewardPreSetup);
    }
}