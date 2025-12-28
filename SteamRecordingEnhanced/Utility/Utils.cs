using System.Linq;
using Dalamud.Game.Text;
using Lumina.Excel.Sheets;
using SteamRecordingEnhanced.Steam;

namespace SteamRecordingEnhanced.Utility;

public static class Utils
{
    public static string GetTerritoryName(ushort territoryTypeId)
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

    public static string GetJobName(uint jobId)
    {
        var jobName = $"UNKNOWN_JOB_{jobId}";
        if (Services.DataManager.GetExcelSheet<ClassJob>().TryGetRow(jobId, out var classJobRow))
        {
            jobName = classJobRow.NameEnglish.ToString();
        }

        return jobName;
    }

    public static string GetPhantomJobName(uint jobId)
    {
        var jobName = $"UNKNOWN_PHANTOM_JOB_{jobId}";
        if (Services.DataManager.GetExcelSheet<MKDSupportJob>().TryGetRow(jobId, out var mkdSupportJobRow))
        {
            jobName = mkdSupportJobRow.NameEnglish.ToString();
        }

        return jobName;
    }

    public static string GetJobAbbreviation(uint jobId)
    {
        var jobName = $"UNKNOWN_JOB_Abbreviation_{jobId}";
        if (Services.DataManager.GetExcelSheet<ClassJob>().TryGetRow(jobId, out var classJobRow))
        {
            jobName = classJobRow.Abbreviation.ToString();
        }

        return jobName;
    }

    public static string? GetIconUrl(this SteamIcon value)
    {
        return value.GetType()?
            .GetField(value.ToString())?
            .GetCustomAttributes(typeof(IconUrlAttribute), false)
            .SingleOrDefault() is IconUrlAttribute attribute
            ? attribute.Url
            : null;
    }

    // Some quest names contains these symbols and there are no good alternatives that steam can render
    // so we just remove them
    public static string ClearSeQuestIcons(string input)
    {
        return input.Replace(SeIconChar.QuestRepeatable.ToIconString(), "")
            .Replace(SeIconChar.QuestSync.ToIconString(), "");
    }
}