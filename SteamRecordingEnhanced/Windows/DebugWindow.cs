using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using SteamRecordingEnhanced.PluginServices.Event;
using SteamRecordingEnhanced.Steam;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.Windows;

public class DebugWindow : Window
{
    public DebugWindow()
        : base("Steam Recording Debug Window")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
    }

    public override unsafe void Draw()
    {
        ImGui.TextUnformatted($"Steam handle by framework {Framework.Instance()->SteamApiLibraryHandle:X}");
        ImGui.TextUnformatted($"CurrentContentFinderConditionId {GameMain.Instance()->CurrentContentFinderConditionId:X}");
        ImGui.TextUnformatted($"ShouldShowName {PvpKillEvent.ShouldShowName()}");

        if (!Services.SteamService.SteamLoaded)
        {
            using var color = ImRaii.PushColor(ImGuiCol.Text, new Vector4(1f, 0, 0, 1f));
            ImGui.TextUnformatted("Steam not loaded");
            return;
        }

        using (ImRaii.PushColor(ImGuiCol.Text, new Vector4(0, 1f, 0, 1f)))
            ImGui.TextUnformatted("Steam loaded");
        var timeline = Services.SteamService.GetSteamTimeline();
        ImGui.TextUnformatted($"Steam timeline: {(IntPtr)timeline:X}");
        if (timeline != null)
        {
            ImGui.Separator();
            DrawTimelineTooltipActions(timeline);
            ImGui.Separator();
            DrawTimelineGameModeAction(timeline);
            ImGui.Separator();
            DrawTimelineEventActions(timeline);
            ImGui.Separator();
            DrawGamePhaseActions(timeline);
            ImGui.Separator();
            if (ImGui.Button("Test event spreading logic"))
            {
                for (uint i = 0; i < 5; i++)
                {
                    Services.TimelineService.AddEvent(i.ToString(), "", $"steam_{i}", i, -5);
                }
            }
        }
    }

    private string tooltipDescription = "tooltipDescription";
    private float timeDelta;

    private unsafe void DrawTimelineTooltipActions(SteamTimeline* timeline)
    {
        using var id = ImRaii.PushId("DrawTimelineTooltipActions");
        ImGui.InputText("Description", ref tooltipDescription);
        ImGui.InputFloat("TimeDelta", ref timeDelta);
        if (ImGui.Button("SetTimelineTooltip"))
        {
            timeline->SetTimelineTooltip(tooltipDescription, timeDelta);
        }

        ImGui.SameLine();
        if (ImGui.Button("ClearTimelineTooltip"))
        {
            timeline->ClearTimelineTooltip(timeDelta);
        }
    }

    private TimelineGameMode gameMode;

    private unsafe void DrawTimelineGameModeAction(SteamTimeline* timeline)
    {
        using var id = ImRaii.PushId("DrawTimelineGameModeAction");
        GuiUtils.Combo("Mode", ref gameMode);
        if (ImGui.Button("SetTimelineGameMode"))
        {
            timeline->SetTimelineGameMode(gameMode);
        }
    }

    private string title = "title";
    private string description = "description";
    private string icon = "steam_bookmark";
    private uint iconPriority;
    private float duration;
    private float offsetSeconds;
    private TimelineEventClipPriority possibleClip = TimelineEventClipPriority.None;
    private ulong eventHandle;
    private ulong eventRecordingExistsCallback;
    private float offsetSecondsInterval;
    private uint eventCount = 1;

    private unsafe void DrawTimelineEventActions(SteamTimeline* timeline)
    {
        using var id = ImRaii.PushId("DrawTimelineEventActions");
        ImGui.InputText("Title", ref title);
        ImGui.InputText("Description", ref description);
        GuiUtils.SteamIconSelect("Icon", ref icon);
        ImGui.InputText("Icon", ref icon);
        ImGui.InputUInt("IconPriority", ref iconPriority);
        ImGui.InputFloat("Duration", ref duration);
        ImGui.InputFloat("OffsetSeconds", ref offsetSeconds);
        GuiUtils.Combo("PossibleClip", ref possibleClip);
        ImGui.InputULong("eventHandle", ref eventHandle);
        ImGui.TextUnformatted($"EventRecordingExistsCallback: {eventRecordingExistsCallback}");
        ImGui.InputFloat("Duration", ref duration);
        ImGui.InputFloat("OffsetSecondsInterval", ref offsetSecondsInterval);
        ImGui.InputUInt("EventCount", ref eventCount);
        if (ImGui.Button("AddInstantaneousTimelineEvent"))
        {
            for (uint i = 0; i < eventCount; i++)
            {
                eventHandle = timeline->AddInstantaneousTimelineEvent(title + i, description + i, icon, iconPriority - i, offsetSeconds + offsetSecondsInterval * i, possibleClip);
            }
        }

        ImGui.SameLine();
        if (ImGui.Button("AddRangeTimelineEvent"))
        {
            eventHandle = timeline->AddRangeTimelineEvent(title, description, icon, iconPriority, offsetSeconds, duration, possibleClip);
        }

        ImGui.SameLine();
        if (ImGui.Button("StartRangeTimelineEvent"))
        {
            eventHandle = timeline->StartRangeTimelineEvent(title, description, icon, iconPriority, offsetSeconds, possibleClip);
        }

        if (ImGui.Button("UpdateRangeTimelineEvent"))
        {
            timeline->UpdateRangeTimelineEvent(eventHandle, title, description, icon, iconPriority, possibleClip);
        }

        ImGui.SameLine();
        if (ImGui.Button("EndRangeTimelineEvent"))
        {
            timeline->EndRangeTimelineEvent(eventHandle, offsetSeconds);
        }

        ImGui.SameLine();
        if (ImGui.Button("RemoveTimelineEvent"))
        {
            timeline->RemoveTimelineEvent(eventHandle);
        }

        if (ImGui.Button("DoesEventRecordingExist"))
        {
            eventRecordingExistsCallback = timeline->DoesEventRecordingExist(eventHandle);
        }

        ImGui.SameLine();
        if (ImGui.Button("OpenOverlayToTimelineEvent"))
        {
            timeline->OpenOverlayToTimelineEvent(eventHandle);
        }
    }

    private string phaseId = "testPhase";
    private string tagName = "tagName";
    private string tagIcon = "";
    private string tagGroup = "tagGroup";
    private string attributeGroup = "attributeGroup";
    private string attributeValue = "attributeValue";
    private uint priority;
    private ulong gamePhaseExistsCallback;


    private unsafe void DrawGamePhaseActions(SteamTimeline* timeline)
    {
        using var id = ImRaii.PushId("DrawGamePhaseActions");
        ImGui.InputText("PhaseId", ref phaseId);
        ImGui.InputText("TagName", ref tagName);
        GuiUtils.SteamIconSelect("TagIcon", ref tagIcon);
        ImGui.InputText("TagIcon", ref tagIcon);
        ImGui.InputText("TagIcon", ref tagIcon);
        ImGui.InputText("TagGroup", ref tagGroup);
        ImGui.InputText("AttributeGroup", ref attributeGroup);
        ImGui.InputText("AttributeValue", ref attributeValue);
        ImGui.InputUInt("Priority", ref priority);
        ImGui.TextUnformatted($"gamePhaseExistsCallback: {gamePhaseExistsCallback}");
        if (ImGui.Button("StartGamePhase"))
        {
            timeline->StartGamePhase();
        }

        ImGui.SameLine();
        if (ImGui.Button("EndGamePhase"))
        {
            timeline->EndGamePhase();
        }

        ImGui.SameLine();
        if (ImGui.Button("SetGamePhaseId"))
        {
            timeline->SetGamePhaseId(phaseId);
        }

        if (ImGui.Button("DoesGamePhaseRecordingExist"))
        {
            gamePhaseExistsCallback = timeline->DoesGamePhaseRecordingExist(phaseId);
        }

        ImGui.SameLine();
        if (ImGui.Button("AddGamePhaseTag"))
        {
            timeline->AddGamePhaseTag(tagName, tagIcon, tagGroup, priority);
        }

        ImGui.SameLine();
        if (ImGui.Button("SetGamePhaseAttribute"))
        {
            timeline->SetGamePhaseAttribute(attributeGroup, attributeValue, priority);
        }

        if (ImGui.Button("OpenOverlayToGamePhase"))
        {
            timeline->OpenOverlayToGamePhase(phaseId);
        }
    }
}
