namespace Chess.Programming.Ago.Game;

using System.ComponentModel;
using System.Reflection;

public static class GameEndReasonExtensions {
    public static string GetDescription(this GameEndReason value) {
        var field = value.GetType().GetField(value.ToString());
        if (field == null) {
            return value.ToString();
        }

        var attribute = field.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }
}

