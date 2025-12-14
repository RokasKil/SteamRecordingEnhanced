namespace SteamRecordingEnhanced.Windows.Tabs;

internal interface ITab
{
    string Title { get; }
    void Draw();
}
