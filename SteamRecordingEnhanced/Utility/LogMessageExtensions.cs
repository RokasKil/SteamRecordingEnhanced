using Dalamud.Game.Chat;

namespace SteamRecordingEnhanced.Utility;

public static class LogMessageExtensions
{
    public static bool IsLocalPlayer(this ILogMessageEntity? messageEntity)
    {
        return Services.PlayerState.IsLoaded && messageEntity is { IsPlayer: true } &&
               Services.PlayerState.CharacterName == messageEntity.Name.ToString() &&
               Services.PlayerState.HomeWorld.RowId == messageEntity.HomeWorldId;
    }

    extension(ILogMessage message)
    {
        public bool TryGetIntParameterWithLog(int i, out int param)
        {
            if (message.TryGetIntParameter(i, out param)) return true;
            Services.Log.Error($"Failed to get {i} param out of {message.DebugFormatLog()}");
            return false;
        }

        public string DebugFormatLog()
        {
            var param = "";
            for (int i = 0; i < message.ParameterCount; i++)
            {
                if (i != 0) param += "; ";
                if (message.TryGetIntParameter(i, out var paramInt))
                {
                    param += $"{paramInt}";
                }
                else if (message.TryGetStringParameter(i, out var paramString))
                {
                    param += $"{paramString}";
                }
                else
                {
                    param += $"UNKONWN_PARAM_{i}";
                }
            }

            return $"[{message.LogMessageId}] {message.SourceEntity?.Name}({message.SourceEntity?.ObjStrId};{message.SourceEntity.IsLocalPlayer()})|" +
                   $"{message.TargetEntity?.Name}({message.TargetEntity?.ObjStrId};{message.TargetEntity.IsLocalPlayer()})|" +
                   $"{param}: {message.FormatLogMessageForDebugging()}";
        }
    }
}