using System;
using System.IO;
using System.Runtime.InteropServices;
using Dalamud.Interface.ImGuiNotification;
using SteamRecordingEnhanced.Steam;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.PluginServices;

public class SteamService : AbstractService
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate uint GetHSteamUserDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate byte SteamApiInitDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate IntPtr FindOrCreateInterfaceDelegate(uint hSteamUser, [MarshalAs(UnmanagedType.LPUTF8Str)] string interfaceVersion);


    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string moduleName);

    private GetHSteamUserDelegate? getHSteamUser;
    private SteamApiInitDelegate? steamApiInit;
    private FindOrCreateInterfaceDelegate? findOrCreateInterface;

    public bool SteamLoaded { get; private set; } = false;

    private IntPtr steamHandle;

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

            steamApiInit = Marshal.GetDelegateForFunctionPointer<SteamApiInitDelegate>(NativeLibrary.GetExport(steamHandle, "SteamAPI_Init"));
            getHSteamUser = Marshal.GetDelegateForFunctionPointer<GetHSteamUserDelegate>(NativeLibrary.GetExport(steamHandle, "SteamAPI_GetHSteamUser"));
            findOrCreateInterface = Marshal.GetDelegateForFunctionPointer<FindOrCreateInterfaceDelegate>(NativeLibrary.GetExport(steamHandle, "SteamInternal_FindOrCreateUserInterface"));

            SteamLoaded = steamApiInit() == 1;
            Services.Log.Information($"Steam api init {SteamLoaded}");
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

        if (!SteamLoaded)
        {
            Services.NotificationManager.AddNotification(new()
            {
                Title = "Steam API Error",
                Content = "Failed to load steam, check configuration for help.",
                Minimized = false,
                Type = NotificationType.Error,
                InitialDuration = TimeSpan.MaxValue
            });
        }
    }

    private IntPtr GetInterface(string interfaceVersion)
    {
        if (!SteamLoaded)
        {
            return IntPtr.Zero;
        }

        uint hSteamUser = getHSteamUser!();
        if (hSteamUser == 0)
        {
            Services.Log.Error("Failed to get HSteamUser");
            return IntPtr.Zero;
        }

        var result = findOrCreateInterface!(hSteamUser, interfaceVersion);
        if (result == IntPtr.Zero)
        {
            Services.Log.Error($"Failed to get instance {interfaceVersion}");
        }

        return result;
    }

    public unsafe SteamTimeline* GetSteamTimeline()
    {
        return (SteamTimeline*)GetInterface(SteamTimeline.INTERFACE_VERSION);
    }

    public unsafe SteamUtils* GetSteamUtils()
    {
        return (SteamUtils*)GetInterface(SteamUtils.INTERFACE_VERSION);
    }

    public unsafe bool? IsOverlayEnabled()
    {
        var steamUtils = GetSteamUtils();
        if (steamUtils != null)
        {
            return steamUtils->IsOverlayEnabled();
        }

        return null;
    }

    public unsafe uint? GetAppId()
    {
        var steamUtils = GetSteamUtils();
        if (steamUtils != null)
        {
            return steamUtils->GetAppId();
        }

        return null;
    }
}