using System.Linq;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using Lumina.Excel.Sheets;
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
                Services.TimelineService.AddEvent("You died", MakeDescriptionString(character), Services.Configuration.PlayerDiedIcon, EventPriorities.PLAYER_DIED_PRIORITY);
            }
            else if (Services.PartyList.Any(partyMember => partyMember.ContentId == (long)character->ContentId))
            {
                Services.TimelineService.AddEvent("Party member died", MakeDescriptionString(character), Services.Configuration.PartyMemberDiedIcon, EventPriorities.PARTY_MEMBER_DIED_PRIORITY);
            }
        }
    }

    private unsafe string MakeDescriptionString(Character* character)
    {
        string job = $"UNKNOWN_JOB_{character->ClassJob}";
        if (Services.DataManager.GetExcelSheet<ClassJob>().TryGetRow(character->ClassJob, out var classJobRow))
        {
            job = classJobRow.Abbreviation.ToString();
        }

        return $"{character->NameString} ({job}) has died!";
    }
}
