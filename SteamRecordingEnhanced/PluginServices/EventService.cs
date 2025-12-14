using System;
using System.Linq;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using Lumina.Excel.Sheets;
using SteamRecordingEnhanced.Steam;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.PluginServices;

public unsafe class EventService : AbstractService
{
    private const uint LevelUpPriority = 8;
    private const uint QuestCompletePriority = 9;
    private const uint AchievementUnlockedPriority = 10;
    private const uint TerritoryChangedPriority = 0;
    private const uint DutyStartedPriority = 5;
    private const uint DutyWipedPriority = 5;
    private const uint DutyCompletePriority = 5;
    private const uint PlayerDiedPriority = 4;
    private const uint PartyMemberDiedPriority = 3;

    private const float MinimumEventInterval = 1f;

    private ulong? combatEventHandle;
    private DateTime lastEvent = DateTime.Now;
    private TimelineGameMode currentGameMode = TimelineGameMode.Invalid;


    delegate void AchievementUnlockedDelegate(IntPtr achievement, uint achievementId);

    delegate void SetQuestCompletionDelegate(ulong questId, byte complete, byte unk3, ulong unk4);

    private Hook<AchievementUnlockedDelegate> achievementUnlockedHook = null!;
    private Hook<SetQuestCompletionDelegate> setQuestCompletionHook = null!;
    private Hook<Character.Delegates.SetMode> characterSetModeHook = null!;


    public override void Init()
    {
        achievementUnlockedHook = Hook<AchievementUnlockedDelegate>("81 FA ?? ?? ?? ?? 0F 87 ?? ?? ?? ?? 53", AchievementUnlockedDetour);
        setQuestCompletionHook = Hook<SetQuestCompletionDelegate>("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 41 56 41 57 48 83 EC ?? 0F B7 E9", SetQuestCompletionDetour);
        characterSetModeHook = Hook<Character.Delegates.SetMode>(Character.Addresses.SetMode.String, CharacterSetModeDetour);

        Services.Condition.ConditionChange += ConditionChange;
        Services.DutyState.DutyStarted += DutyStarted;
        Services.DutyState.DutyWiped += DutyWiped;
        Services.DutyState.DutyCompleted += DutyCompleted;
        Services.ClientState.TerritoryChanged += TerritoryChanged;
        Services.ClientState.LevelChanged += LevelChanged;
        Services.Framework.Update += Tick;

        if (Services.ClientState.IsLoggedIn)
        {
            StartPhase();
            if (Services.Condition[ConditionFlag.InCombat])
            {
                StartCombatEvent();
            }
        }

        EnableHooks();
    }

    private void Tick(IFramework framework)
    {
        var gameMode = Services.ClientState.IsLoggedIn ? TimelineGameMode.Playing : TimelineGameMode.Menus;
        if (Services.GameGui.GetAddonByName("NowLoading").IsVisible)
        {
            gameMode = TimelineGameMode.LoadingScreen;
        }

        if (currentGameMode != gameMode)
        {
            SetGameMode(gameMode);
        }
    }

    private SteamTimeline* GetTimeline()
    {
        if (Services.SteamService.SteamLoaded)
        {
            SteamTimeline* timeline = Services.SteamService.GetSteamTimeline();
            if (timeline == null)
            {
                Services.Log.Error("Steam is loaded, but failed to get timeline");
            }

            return timeline;
        }

        return null;
    }

    private void AddEvent(string title, string description, string icon, uint priority = 0)
    {
        if (icon != "")
        {
            var timeline = GetTimeline();
            if (timeline != null)
            {
                // Stagger the events slightly so they don't fully overlap in cases like completing a quest and leveling up   
                lastEvent = lastEvent.AddSeconds(MinimumEventInterval);
                lastEvent = lastEvent > DateTime.Now ? lastEvent : DateTime.Now;
                var offset = (lastEvent - DateTime.Now).Milliseconds / 1000f;
                timeline->AddInstantaneousTimelineEvent(title, description, icon, priority, offset);
            }
        }
    }

    private void SetGameMode(TimelineGameMode gameMode)
    {
        var timeline = GetTimeline();
        if (timeline != null)
        {
            Services.Log.Info($"Setting gamemode {gameMode}");
            timeline->SetTimelineGameMode(gameMode);
            currentGameMode = gameMode;
        }
    }

    private void StartCombatEvent()
    {
        StopCombatEvent();
        if (!Services.Configuration.HighlightCombat)
        {
            return;
        }

        var timeline = GetTimeline();
        if (timeline != null)
        {
            combatEventHandle = timeline->StartRangeTimelineEvent("Combat", "Combat description", "steam_combat");
        }
    }


    private void StopCombatEvent()
    {
        if (combatEventHandle.HasValue)
        {
            var timeline = GetTimeline();
            if (timeline != null)
            {
                timeline->EndRangeTimelineEvent(combatEventHandle.Value);
            }

            combatEventHandle = null;
        }
    }

    private void StartPhase()
    {
        EndPhase();
        var timeline = GetTimeline();
        if (timeline != null)
        {
            string world = Services.PlayerState.HomeWorld.ValueNullable?.Name.ToString() ?? "UNKNOWN_WORLD";
            string name = $"{Services.PlayerState.CharacterName}@{world}";
            string territory = GetTerritoryName(Services.ClientState.TerritoryType);

            timeline->StartGamePhase();
            // Non searchable info
            timeline->SetGamePhaseAttribute("name", name, 1);
            timeline->SetGamePhaseAttribute("territory", territory, 0);
            // Searchable info that only shows the icon if it's set for some reason?
            timeline->AddGamePhaseTag(name + " " + territory, "steam_person", "search_tag");

            // Timeline tooltip
            timeline->SetTimelineTooltip(territory);
        }
    }

    private void EndPhase()
    {
        var timeline = GetTimeline();
        if (timeline != null)
        {
            timeline->EndGamePhase();

            // Timeline tooltip
            timeline->ClearTimelineTooltip();
        }
    }

    private void TerritoryChanged(ushort territoryTypeId)
    {
        AddEvent("Territory changed", GetTerritoryName(territoryTypeId), Services.Configuration.TerritoryChangedIcon, TerritoryChangedPriority);
        StartPhase();
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


    private void DutyStarted(object? sender, ushort e)
    {
        AddEvent("Duty started", $"DUTY NAME {e}", Services.Configuration.DutyStartedIcon, DutyStartedPriority);
    }

    private void DutyCompleted(object? sender, ushort e)
    {
        AddEvent("Duty completed", $"DUTY COMPLETED {e}", Services.Configuration.DutyCompleteIcon, DutyCompletePriority);
    }

    private void DutyWiped(object? sender, ushort e)
    {
        AddEvent("Duty wiped", $"DUTY WIPED {e}", Services.Configuration.DutyWipedIcon, DutyWipedPriority);
    }

    private void AchievementUnlockedDetour(IntPtr achievement, uint achievementId)
    {
        achievementUnlockedHook.Original(achievement, achievementId);

        var achievementName = $"UNKNOWN_ACHIEVEMENT_{achievementId}";
        if (Services.DataManager.GetExcelSheet<Achievement>().TryGetRow(achievementId, out var achievementRow))
        {
            achievementName = achievementRow.Name.ToString();
        }

        AddEvent("Achievement unlocked", $"{achievementName}", Services.Configuration.AchievementUnlockedIcon, AchievementUnlockedPriority);
    }

    private string GetTerritoryName(ushort territoryTypeId)
    {
        var territory = $"UNKNOWN_TERRITORY_{territoryTypeId}";
        if (Services.DataManager.GetExcelSheet<TerritoryType>().TryGetRow(territoryTypeId, out var territoryRow))
        {
            var region = territoryRow.PlaceNameRegion.Value.Name;
            territory = region != "" ? $"{region} > " : "";
            territory += territoryRow.PlaceName.Value.Name.ToString();
        }

        return territory;
    }

    private void LevelChanged(uint classJobId, uint level)
    {
        var jobName = $"UNKNOWN_JOB_{classJobId}";
        if (Services.DataManager.GetExcelSheet<ClassJob>().TryGetRow(classJobId, out var classJobRow))
        {
            jobName = classJobRow.NameEnglish.ToString();
        }

        AddEvent("Level up", $"{jobName} Lv. {level}", Services.Configuration.AchievementUnlockedIcon, LevelUpPriority);
    }


    private void SetQuestCompletionDetour(ulong questId, byte complete, byte unk3, ulong unk4)
    {
        // the native method has a few checks related to ng+ and some other stuff I didn't dig into
        // it also seems to be capable of uncompleting quests so let's just let the method do it's work
        // and check the state of the quest before and after
        Services.Log.Debug($"SetQuestCompletionDetour {questId} {complete} {unk3} {unk4}");
        // the native method does this too
        var id = (ushort)(questId & 0xffff);
        var completeBefore = QuestManager.IsQuestComplete(id);
        setQuestCompletionHook.Original(questId, complete, unk3, unk4);
        if (!completeBefore && QuestManager.IsQuestComplete(id))
        {
            var questName = $"UNKNOWN_QUEST_{id}";
            if (Services.DataManager.GetExcelSheet<Quest>().TryGetRow(id + 0x10000u, out var classJobRow))
            {
                questName = classJobRow.Name.ToString();
            }

            AddEvent("Quest complete", questName, Services.Configuration.QuestCompleteIcon, QuestCompletePriority);
        }
    }

    private void CharacterSetModeDetour(Character* character, CharacterModes mode, byte modeParam)
    {
        var isDeadBefore = character->IsDead();
        characterSetModeHook.Original(character, mode, modeParam);
        if (!isDeadBefore && character->IsDead())
        {
            if (character->ContentId == Services.PlayerState.ContentId)
            {
                AddEvent("You died", $"{character->NameString} has died!", Services.Configuration.PlayerDiedIcon, PlayerDiedPriority);
            }
            else if (Services.PartyList.Any(partyMember => partyMember.ContentId == (long)character->ContentId))
            {
                AddEvent("Party member died", $"{character->NameString} has died!", Services.Configuration.PartyMemberDiedIcon, PartyMemberDiedPriority);
            }
        }
    }

    public override void Dispose()
    {
        StopCombatEvent();
        EndPhase();
        SetGameMode(TimelineGameMode.LoadingScreen); // The steam default
        base.Dispose();
    }
}
