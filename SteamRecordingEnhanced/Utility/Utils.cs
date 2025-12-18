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
}
