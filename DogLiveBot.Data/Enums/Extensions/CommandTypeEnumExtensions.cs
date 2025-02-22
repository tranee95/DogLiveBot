using System.ComponentModel;
using System.Reflection;

namespace DogLiveBot.Data.Enums.Extensions;

public static class CommandTypeEnumExtensions
{
    public static CommandTypeEnum GetCommandTypeEnum(this CommandTypeEnum commandTypeEnum, string commandTextType)
    {
        var type = commandTypeEnum.GetType();
        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            if (attribute != null && attribute.Description.Equals(commandTextType, StringComparison.OrdinalIgnoreCase))
            {
                return (CommandTypeEnum)field.GetValue(null);
            }
        }

        return CommandTypeEnum.Start;
    }
}