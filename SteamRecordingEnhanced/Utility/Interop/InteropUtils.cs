using System.Text;

namespace SteamRecordingEnhanced.Utility.Interop;

public static class InteropUtils
{
    public static byte[] StringToUtf8Bytes(string str)
    {
        return Encoding.UTF8.GetBytes(str + "\0");
    }
}
