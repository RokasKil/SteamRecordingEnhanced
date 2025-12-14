using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using SteamRecordingEnhanced.Interop;
using SteamRecordingEnhanced.Steam;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.PluginServices;

public class SteamService : AbstractService
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate uint GetHSteamUserDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate byte SteamApiInitDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate IntPtr FindOrCreateInterfaceDelegate(uint hSteamUser, [MarshalAs(UnmanagedType.LPUTF8Str)] string interfaceVersion);


    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string moduleName);

    private IntPtr steamHandle;

    public GetHSteamUserDelegate? GetHSteamUser;
    public SteamApiInitDelegate? SteamApiInit;
    public FindOrCreateInterfaceDelegate? FindOrCreateInterface;

    public bool SteamLoaded { get; private set; } = false;

    public override void Init()
    {
        try
        {
            steamHandle = GetModuleHandle("steam_api64.dll");
            Services.Log.Debug($"Got steam handle {steamHandle}");
            if (steamHandle == IntPtr.Zero)
            {
                Services.Log.Debug($"Steam wasn't loaded, trying to load manually");
                steamHandle = NativeLibrary.Load(Path.GetFullPath("../../boot/steam_api64.dll", Environment.ProcessPath!));
                Services.Log.Debug($"Got steam handle2 {steamHandle}");
            }

            SteamApiInit = Marshal.GetDelegateForFunctionPointer<SteamApiInitDelegate>(NativeLibrary.GetExport(steamHandle, "SteamAPI_Init"));
            GetHSteamUser = Marshal.GetDelegateForFunctionPointer<GetHSteamUserDelegate>(NativeLibrary.GetExport(steamHandle, "SteamAPI_GetHSteamUser"));
            FindOrCreateInterface = Marshal.GetDelegateForFunctionPointer<FindOrCreateInterfaceDelegate>(NativeLibrary.GetExport(steamHandle, "SteamInternal_FindOrCreateUserInterface"));

            SteamLoaded = SteamApiInit() == 1;
            Services.Log.Debug($"Steam api init {SteamLoaded}");
        }
        catch (Exception ex)
        {
            if (ex is ArgumentNullException or EntryPointNotFoundException)
            {
                Services.Log.Error(ex, "Failed to load steam service");
                return;
            }

            throw;
        }
    }

    public unsafe SteamTimeline* GetSteamTimeline()
    {
        if (!SteamLoaded)
        {
            return null;
        }

        uint hSteamUser = GetHSteamUser!();
        if (hSteamUser == 0)
        {
            return null;
        }

        return (SteamTimeline*)FindOrCreateInterface!(hSteamUser, SteamTimeline.INTERFACE_VERSION);
    }
}
