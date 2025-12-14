using System.Runtime.InteropServices;
using SteamRecordingEnhanced.Utility.Interop;

namespace SteamRecordingEnhanced.Steam;

// Based on https://github.com/rlabrecque/SteamworksSDK/blob/main/public/steam/isteamtimeline.h
[StructLayout(LayoutKind.Sequential)]
public unsafe struct SteamTimeline
{
    public const string INTERFACE_VERSION = "STEAMTIMELINE_INTERFACE_V004";
    public SteamTimelineVTable* VTable;


    public void SetTimelineTooltip(string description, float timeDelta = 0f)
    {
        fixed (SteamTimeline* thisPtr = &this)
        fixed (byte* descPtr = InteropUtils.StringToUtf8Bytes(description))
            VTable->SetTimelineTooltip(thisPtr, descPtr, timeDelta);
    }

    public void ClearTimelineTooltip(float timeDelta = 0f)
    {
        fixed (SteamTimeline* thisPtr = &this)
            VTable->ClearTimelineTooltip(thisPtr, timeDelta);
    }

    public void SetTimelineGameMode(TimelineGameMode mode)
    {
        fixed (SteamTimeline* thisPtr = &this)
            VTable->SetTimelineGameMode(thisPtr, mode);
    }

    public ulong AddInstantaneousTimelineEvent(string title, string description, string icon, uint iconPriority = 0, float startOffsetSeconds = 0f, TimelineEventClipPriority possibleClip = TimelineEventClipPriority.None)
    {
        fixed (SteamTimeline* thisPtr = &this)
        fixed (byte* titlePtr = InteropUtils.StringToUtf8Bytes(title))
        fixed (byte* descPtr = InteropUtils.StringToUtf8Bytes(description))
        fixed (byte* iconPtr = InteropUtils.StringToUtf8Bytes(icon))
            return VTable->AddInstantaneousTimelineEvent(thisPtr, titlePtr, descPtr, iconPtr, iconPriority, startOffsetSeconds, possibleClip);
    }

    public ulong AddRangeTimelineEvent(string title, string description, string icon, uint iconPriority = 0, float startOffsetSeconds = 0f, float duration = 0f, TimelineEventClipPriority possibleClip = TimelineEventClipPriority.None)
    {
        fixed (SteamTimeline* thisPtr = &this)
        fixed (byte* titlePtr = InteropUtils.StringToUtf8Bytes(title))
        fixed (byte* descPtr = InteropUtils.StringToUtf8Bytes(description))
        fixed (byte* iconPtr = InteropUtils.StringToUtf8Bytes(icon))
            return VTable->AddRangeTimelineEvent(thisPtr, titlePtr, descPtr, iconPtr, iconPriority, startOffsetSeconds, duration, possibleClip);
    }

    public ulong StartRangeTimelineEvent(string title, string description, string icon, uint priority = 0, float startOffsetSeconds = 0f, TimelineEventClipPriority possibleClip = TimelineEventClipPriority.None)
    {
        fixed (SteamTimeline* thisPtr = &this)
        fixed (byte* titlePtr = InteropUtils.StringToUtf8Bytes(title))
        fixed (byte* descPtr = InteropUtils.StringToUtf8Bytes(description))
        fixed (byte* iconPtr = InteropUtils.StringToUtf8Bytes(icon))
            return VTable->StartRangeTimelineEvent(thisPtr, titlePtr, descPtr, iconPtr, priority, startOffsetSeconds, possibleClip);
    }

    public void UpdateRangeTimelineEvent(ulong eventHandle, string title, string description, string icon, uint priority = 0, TimelineEventClipPriority possibleClip = TimelineEventClipPriority.None)
    {
        fixed (SteamTimeline* thisPtr = &this)
        fixed (byte* titlePtr = InteropUtils.StringToUtf8Bytes(title))
        fixed (byte* descPtr = InteropUtils.StringToUtf8Bytes(description))
        fixed (byte* iconPtr = InteropUtils.StringToUtf8Bytes(icon))
            VTable->UpdateRangeTimelineEvent(thisPtr, eventHandle, titlePtr, descPtr, iconPtr, priority, possibleClip);
    }

    public void EndRangeTimelineEvent(ulong eventHandle, float endOffsetSeconds = 0)
    {
        fixed (SteamTimeline* thisPtr = &this)
            VTable->EndRangeTimelineEvent(thisPtr, eventHandle, endOffsetSeconds);
    }

    public void RemoveTimelineEvent(ulong eventHandle)
    {
        fixed (SteamTimeline* thisPtr = &this)
            VTable->RemoveTimelineEvent(thisPtr, eventHandle);
    }

    public ulong DoesEventRecordingExist(ulong eventHandle)
    {
        fixed (SteamTimeline* thisPtr = &this)
            return VTable->DoesEventRecordingExist(thisPtr, eventHandle);
    }

    public void StartGamePhase()
    {
        fixed (SteamTimeline* thisPtr = &this)
            VTable->StartGamePhase(thisPtr);
    }

    public void EndGamePhase()
    {
        fixed (SteamTimeline* thisPtr = &this)
            VTable->EndGamePhase(thisPtr);
    }

    public void SetGamePhaseId(string phaseId)
    {
        fixed (SteamTimeline* thisPtr = &this)
        fixed (byte* phaseIdPtr = InteropUtils.StringToUtf8Bytes(phaseId))
            VTable->SetGamePhaseId(thisPtr, phaseIdPtr);
    }

    public ulong DoesGamePhaseRecordingExist(string phaseId)
    {
        fixed (SteamTimeline* thisPtr = &this)
        fixed (byte* phaseIdPtr = InteropUtils.StringToUtf8Bytes(phaseId))
            return VTable->DoesGamePhaseRecordingExist(thisPtr, phaseIdPtr);
    }

    public void AddGamePhaseTag(string tagName, string tagIcon, string tagGroup, uint priority = 0)
    {
        fixed (SteamTimeline* thisPtr = &this)
        fixed (byte* namePtr = InteropUtils.StringToUtf8Bytes(tagName))
        fixed (byte* iconPtr = InteropUtils.StringToUtf8Bytes(tagIcon))
        fixed (byte* groupPtr = InteropUtils.StringToUtf8Bytes(tagGroup))
            VTable->AddGamePhaseTag(thisPtr, namePtr, iconPtr, groupPtr, priority);
    }

    public void SetGamePhaseAttribute(string attributeGroup, string attributeValue, uint priority = 0)
    {
        fixed (SteamTimeline* thisPtr = &this)
        fixed (byte* groupPtr = InteropUtils.StringToUtf8Bytes(attributeGroup))
        fixed (byte* valuePtr = InteropUtils.StringToUtf8Bytes(attributeValue))
            VTable->SetGamePhaseAttribute(thisPtr, groupPtr, valuePtr, priority);
    }

    public void OpenOverlayToGamePhase(string phaseId)
    {
        fixed (SteamTimeline* thisPtr = &this)
        fixed (byte* phaseIdPtr = InteropUtils.StringToUtf8Bytes(phaseId))
            VTable->OpenOverlayToGamePhase(thisPtr, phaseIdPtr);
    }

    public void OpenOverlayToTimelineEvent(ulong eventHandle)
    {
        fixed (SteamTimeline* thisPtr = &this)
            VTable->OpenOverlayToTimelineEvent(thisPtr, eventHandle);
    }
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct SteamTimelineVTable
{
    public delegate* unmanaged<SteamTimeline*, byte*, float, void> SetTimelineTooltip;
    public delegate* unmanaged <SteamTimeline*, float, void> ClearTimelineTooltip;
    public delegate* unmanaged <SteamTimeline*, TimelineGameMode, void> SetTimelineGameMode;
    public delegate* unmanaged <SteamTimeline*, byte*, byte*, byte*, uint, float, TimelineEventClipPriority, ulong> AddInstantaneousTimelineEvent;
    public delegate* unmanaged <SteamTimeline*, byte*, byte*, byte*, uint, float, float, TimelineEventClipPriority, ulong> AddRangeTimelineEvent;
    public delegate* unmanaged <SteamTimeline*, byte*, byte*, byte*, uint, float, TimelineEventClipPriority, ulong> StartRangeTimelineEvent;
    public delegate* unmanaged <SteamTimeline*, ulong, byte*, byte*, byte*, uint, TimelineEventClipPriority, void> UpdateRangeTimelineEvent;
    public delegate* unmanaged <SteamTimeline*, ulong, float, void> EndRangeTimelineEvent;
    public delegate* unmanaged <SteamTimeline*, ulong, void> RemoveTimelineEvent;
    public delegate* unmanaged <SteamTimeline*, ulong, ulong> DoesEventRecordingExist;
    public delegate* unmanaged <SteamTimeline*, void> StartGamePhase;
    public delegate* unmanaged <SteamTimeline*, void> EndGamePhase;
    public delegate* unmanaged <SteamTimeline*, byte*, void> SetGamePhaseId;
    public delegate* unmanaged <SteamTimeline*, byte*, ulong> DoesGamePhaseRecordingExist;
    public delegate* unmanaged <SteamTimeline*, byte*, byte*, byte*, uint, void> AddGamePhaseTag;
    public delegate* unmanaged <SteamTimeline*, byte*, byte*, uint, void> SetGamePhaseAttribute;
    public delegate* unmanaged <SteamTimeline*, byte*, void> OpenOverlayToGamePhase;
    public delegate* unmanaged <SteamTimeline*, ulong, void> OpenOverlayToTimelineEvent;
}

public enum TimelineGameMode
{
    Invalid = 0,
    Playing = 1,
    Staging = 2,
    Menus = 3,
    LoadingScreen = 4,
    Max
}

public enum TimelineEventClipPriority
{
    Invalid = 0,
    None = 1,
    Standard = 2,
    Featured = 3
}
