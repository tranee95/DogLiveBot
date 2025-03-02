using System.ComponentModel;

namespace DogLiveBot.Data.Enums.Extensions;

public static class CommandTypeEnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute =
            (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

        return attribute != null ? attribute.Description : value.ToString();
    }
}