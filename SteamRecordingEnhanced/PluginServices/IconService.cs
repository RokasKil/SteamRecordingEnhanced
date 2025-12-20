using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Interface.ManagedFontAtlas;
using SteamRecordingEnhanced.Utility;

namespace SteamRecordingEnhanced.PluginServices;

public class IconService : AbstractService
{
    private readonly HttpClient httpClient = new();
    private readonly DirectoryInfo iconDirectory;
    private readonly ConcurrentSet<string> cachedIcons = [];
    private readonly ConcurrentSet<string> failedIcons = [];
    private readonly ConcurrentSet<string> downloadingIcons = [];
    private readonly CancellationTokenSource disposeCts = new();

    public IFontHandle IconFont { get; init; }

    public IconService()
    {
        iconDirectory = Services.PluginInterface.ConfigDirectory.CreateSubdirectory("icons");
        foreach (var file in iconDirectory.EnumerateFiles())
        {
            cachedIcons.Add(file.Name);
        }

        IconFont = Services.PluginInterface.UiBuilder.FontAtlas
                           .NewDelegateFontHandle(e =>
                                                      e.OnPreBuild(tk =>
                                                                       tk.AddDalamudDefaultFont(Services.PluginInterface.UiBuilder.FontDefaultSizePx / GuiUtils.BASE_FONT_SIZE * GuiUtils.ICON_SIZE)));
    }

    public string? GetIconPath(string iconName, string? url)
    {
        if (cachedIcons.Contains(iconName))
        {
            return Path.Join(iconDirectory.FullName, iconName);
        }
        else if (!failedIcons.Contains(iconName) && url != null)
        {
            CacheIcon(iconName, url);
        }

        return null;
    }

    private void CacheIcon(string iconName, string url)
    {
        if (downloadingIcons.Contains(iconName))
        {
            return;
        }

        downloadingIcons.Add(iconName);
        _ = DownloadAndCacheAsync(iconName, url, disposeCts.Token);
    }

    // I'll be real, I have almost no clue what I'm doing with Task stuff
    private async Task DownloadAndCacheAsync(string iconName, string url, CancellationToken token)
    {
        Services.Log.Debug($"Caching icon {iconName} {url}");
        try
        {
            using var response = await httpClient.GetAsync(url, token).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var bytes = await response.Content.ReadAsByteArrayAsync(token).ConfigureAwait(false);

            var path = Path.Join(iconDirectory.FullName, iconName);
            await File.WriteAllBytesAsync(path, bytes, token).ConfigureAwait(false);

            cachedIcons.Add(iconName);
        }
        catch (OperationCanceledException)
        {
            // Expected during Dispose()
        }
        catch (Exception e)
        {
            Services.Log.Error(e, $"Failed to cache icon {iconName} {url}");
            failedIcons.Add(iconName);
        } finally
        {
            downloadingIcons.Remove(iconName);
        }
    }

    public override void Dispose()
    {
        disposeCts.Cancel();
        disposeCts.Dispose();
        httpClient.Dispose();
        IconFont.Dispose();
        base.Dispose();
    }
}
