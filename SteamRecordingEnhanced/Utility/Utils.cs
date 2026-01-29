using System;
using System.Linq;
using Dalamud.Game.Text;
using Dalamud.Utility;
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
            territory = territoryRow.PlaceName.Value.Name.ToString();
        }

        return territory;
    }

    public static string? GetContentName(ushort territoryTypeId)
    {
        if (Services.DataManager.GetExcelSheet<TerritoryType>().TryGetRow(territoryTypeId, out var territoryRow)
            && territoryRow.ContentFinderCondition.IsValid
            && territoryRow.ContentFinderCondition.RowId != 0)
        {
            var contentName = territoryRow.ContentFinderCondition.Value.Name.ToString().Capitalize();
            // Saw some empty names in the sheet, don't know on what duties they reference to but  in that case just
            // return null so the code can fallback to territory name
            return contentName.IsNullOrWhitespace() ? null : contentName;
        }

        return null;
    }

    public static string GetContentOrTerritoryName(ushort territoryTypeId) => GetContentName(territoryTypeId) ?? GetTerritoryName(territoryTypeId);

    public static string GetJobName(uint jobId)
    {
        var jobName = $"UNKNOWN_JOB_{jobId}";
        if (Services.DataManager.GetExcelSheet<ClassJob>().TryGetRow(jobId, out var classJobRow))
            jobName = classJobRow.NameEnglish.ToString();

        return jobName;
    }

    public static string GetPhantomJobName(uint jobId)
    {
        var jobName = $"UNKNOWN_PHANTOM_JOB_{jobId}";
        if (Services.DataManager.GetExcelSheet<MKDSupportJob>().TryGetRow(jobId, out var mkdSupportJobRow))
            jobName = mkdSupportJobRow.NameEnglish.ToString();

        return jobName;
    }

    public static string GetJobAbbreviation(uint jobId)
    {
        var jobName = $"UNKNOWN_JOB_Abbreviation_{jobId}";
        if (Services.DataManager.GetExcelSheet<ClassJob>().TryGetRow(jobId, out var classJobRow))
            jobName = classJobRow.Abbreviation.ToString();

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
        return input
            .Replace(SeIconChar.QuestRepeatable.ToIconString(), "")
            .Replace(SeIconChar.QuestSync.ToIconString(), "");
    }

    public static string Capitalize(this string input)
    {
        return input.Length > 0 ? string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1)) : input;
    }
}