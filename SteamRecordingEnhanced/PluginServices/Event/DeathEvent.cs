using System.Linq;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using SteamRecordingEnhanced.PluginServices.Event.Metadata;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.PluginServices.Event;

public class DeathEvent : AbstractEvent
{
    private readonly Hook<Character.Delegates.SetMode> characterSetModeHook;

    public DeathEvent()
    {
        unsafe
        {
            characterSetModeHook = Hook<Character.Delegates.SetMode>(Character.Addresses.SetMode.String, CharacterSetModeDetour);
        }

        EnableHooks();
    }

    private unsafe void CharacterSetModeDetour(Character* character, CharacterModes mode, byte modeParam)
    {
        var isDeadBefore = character->IsDead();
        characterSetModeHook.Original(character, mode, modeParam);
        if (!isDeadBefore && character->IsDead())
        {
            if (character->ContentId == Services.PlayerState.ContentId)
            {
                Services.TimelineService.AddEvent("You died", MakeDescriptionString(character), GameEvent.PlayerDied);
            }
            else if (Services.PartyList.Any(partyMember => partyMember.ContentId == (long)character->ContentId))
            {
                Services.TimelineService.AddEvent("Party member died", MakeDescriptionString(character), GameEvent.PartyMemberDied);
            }
        }
    }

    private unsafe string MakeDescriptionString(Character* character)
    {
        return $"{character->NameString} ({Utils.GetJobAbbreviation(character->ClassJob)}) has died!";
    }
}