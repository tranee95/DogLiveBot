using System.ComponentModel;
using System.Reflection;

namespace DogLiveBot.Data.Enums.Helpers;

public static class CommandTypeEnumHelper
{
    /// <summary>
    /// Получает значение перечисления <see cref="CommandTypeEnum"/> по заданной строке описания команды.
    /// </summary>
    /// <param name="commandTextType">Строка, представляющая описание команды, для которой необходимо получить значение перечисления.</param>
    /// <returns>
    /// Возвращает значение <see cref="CommandTypeEnum"/>, соответствующее заданному описанию команды,
    /// или <c>null</c>, если описание не найдено.
    /// </returns>
    public static CommandTypeEnum? GetCommandTypeEnum(string? commandTextType)
    {
        var type = typeof(CommandTypeEnum);
        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            if (attribute != null && attribute.Description.Equals(commandTextType, StringComparison.OrdinalIgnoreCase))
            {
                return (CommandTypeEnum)field.GetValue(null);
            }
        }

        return null;
    }
}