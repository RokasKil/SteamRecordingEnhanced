using System;
using System.Collections.Generic;
using System.Linq;
using SteamRecordingEnhanced.Steam;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.PluginServices;

public unsafe class TimelineService : AbstractService
{
    private const float MinimumEventInterval = 1f;
    public TimelineGameMode CurrentGameMode { get; private set; } = TimelineGameMode.Invalid;
    private readonly SortedSet<DateTime> eventDates = [];


    public void AddEvent(string title, string description, string icon, uint priority = 0, float desiredOffset = 0f)
    {
        if (icon == "")
        {
            return;
        }

        title = PrepareString(title)!;
        description = PrepareString(description)!;
        var timeline = Services.SteamService.GetSteamTimeline();
        if (timeline == null)
        {
            return;
        }

        // do some math to make sure nothing overlaps too much and events don't get eaten
        var desiredDate = DateTime.Now.AddSeconds(desiredOffset);
        foreach (var eventDate in eventDates.GetViewBetween(desiredDate.AddSeconds(-MinimumEventInterval), DateTime.MaxValue))
        {
            if ((eventDate - desiredDate).TotalSeconds <= MinimumEventInterval)
            {
                desiredDate = eventDate.AddSeconds(MinimumEventInterval);
            }
            else
            {
                break;
            }
        }

        eventDates.Add(desiredDate);

        var offset = (float)(desiredDate - DateTime.Now).TotalSeconds;
        Services.Log.Debug($"Adding event {title} {description} {icon} {priority} {offset}");
        timeline->AddInstantaneousTimelineEvent(title, description, icon, priority, offset);

        if (eventDates.Count > 100)
        {
            eventDates.Remove(eventDates.First());
        }
    }

    public ulong? StartEvent(string title, string description, string icon)
    {
        title = PrepareString(title)!;
        description = PrepareString(description)!;
        var timeline = Services.SteamService.GetSteamTimeline();
        if (timeline != null)
        {
            Services.Log.Debug($"Starting event event {title} {description} {icon}");
            return timeline->StartRangeTimelineEvent(title, description, icon);
        }

        return null;
    }

    public void EndEvent(ulong eventHandle)
    {
        var timeline = Services.SteamService.GetSteamTimeline();
        if (timeline != null)
        {
            Services.Log.Debug($"Ending event {eventHandle}");
            timeline->EndRangeTimelineEvent(eventHandle);
        }
    }

    public void SetGameMode(TimelineGameMode gameMode)
    {
        var timeline = Services.SteamService.GetSteamTimeline();
        if (timeline != null)
        {
            timeline->SetTimelineGameMode(gameMode);
        }

        CurrentGameMode = gameMode;
    }


    public void StartGamePhase()
    {
        var timeline = Services.SteamService.GetSteamTimeline();
        if (timeline != null)
        {
            timeline->StartGamePhase();
        }
    }

    public void EndGamePhase()
    {
        var timeline = Services.SteamService.GetSteamTimeline();
        if (timeline != null)
        {
            timeline->EndGamePhase();
        }
    }


    public void SetGamePhaseAttribute(string attributeGroup, string attributeValue, uint priority = 0)
    {
        attributeValue = PrepareString(attributeValue)!;
        var timeline = Services.SteamService.GetSteamTimeline();
        if (timeline != null)
        {
            timeline->SetGamePhaseAttribute(attributeGroup, attributeValue, priority);
        }
    }

    public void AddGamePhaseTag(string tagName, string tagIcon, string tagGroup, uint priority = 0)
    {
        tagName = PrepareString(tagName)!;
        var timeline = Services.SteamService.GetSteamTimeline();
        if (timeline != null)
        {
            timeline->AddGamePhaseTag(tagName, tagIcon, tagGroup, priority);
        }
    }

    public void SetTimelineTooltip(string? tooltip)
    {
        tooltip = PrepareString(tooltip);
        var timeline = Services.SteamService.GetSteamTimeline();
        if (timeline != null)
        {
            if (string.IsNullOrWhiteSpace(tooltip))
            {
                timeline->ClearTimelineTooltip();
            }
            else
            {
                timeline->SetTimelineTooltip(tooltip);
            }
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        EndGamePhase();
        SetTimelineTooltip(null);
        SetGameMode(TimelineGameMode.LoadingScreen); // The steam default
    }

    public void OpenOverlayToGamePhase(string phaseId = "")
    {
        var timeline = Services.SteamService.GetSteamTimeline();
        if (timeline != null)
        {
            timeline->OpenOverlayToGamePhase(phaseId);
        }
    }

    private string? PrepareString(string? input) => input != null ? Utils.ClearSeQuestIcons(input) : input;
}