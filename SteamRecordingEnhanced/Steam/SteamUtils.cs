using System.Runtime.InteropServices;
using SteamRecordingEnhanced.Utility.Interop;

namespace SteamRecordingEnhanced.Steam;

// Based on SteamworksSDK
[StructLayout(LayoutKind.Sequential)]
public unsafe struct SteamUtils
{
    public const string INTERFACE_VERSION = "SteamUtils010";
    public SteamUtilsVTable* VTable;


    public uint GetAppId()
    {
        fixed (SteamUtils* thisPtr = &this)
            return VTable->GetAppId(thisPtr);
    }

    public bool IsOverlayEnabled()
    {
        fixed (SteamUtils* thisPtr = &this)
            return VTable->IsOverlayEnabled(thisPtr);
    }
}

[StructLayout(LayoutKind.Explicit)]
public unsafe struct SteamUtilsVTable
{
    [FieldOffset(8 * 9)]
    public delegate* unmanaged<SteamUtils*, uint> GetAppId;
    [FieldOffset(8 * 17)]
    public delegate* unmanaged <SteamUtils*, bool> IsOverlayEnabled;
}
