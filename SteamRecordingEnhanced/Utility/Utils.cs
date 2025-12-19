using System;
using System.Linq;
using Lumina.Excel.Sheets;

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

    public static string GetJobAbbreviation(uint jobId)
    {
        var jobName = $"UNKNOWN_JOB_Abbreviation_{jobId}";
        if (Services.DataManager.GetExcelSheet<ClassJob>().TryGetRow(jobId, out var classJobRow))
        {
            jobName = classJobRow.Abbreviation.ToString();
        }

        return jobName;
    }

    public static string? GetIconUrl(this Enum value)
    {
        return value.GetType()?
                    .GetField(value.ToString())?
                    .GetCustomAttributes(typeof(IconUrlAttribute), false)
                    .SingleOrDefault() is IconUrlAttribute attribute
                   ? attribute.Url
                   : null;
    }
}
