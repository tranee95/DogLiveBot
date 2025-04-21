using System.ComponentModel;
using System.Reflection;
using Newtonsoft.Json;
using JsonException = System.Text.Json.JsonException;

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
    
    /// <summary>
    /// Пытается десериализовать JSON-строку в объект указанного типа.
    /// </summary>
    /// <typeparam name="T">Тип объекта, в который нужно десериализовать JSON.</typeparam>
    /// <param name="json">JSON-строка для десериализации.</param>
    /// <param name="result">Результат десериализации, если она прошла успешно.</param>
    /// <returns>True, если десериализация прошла успешно, иначе False.</returns>
    public static bool TryParseFromJsonToObject<T>(string json, out T result)
    {
        try
        {
            result = JsonConvert.DeserializeObject<T>(json);
            return result != null;
        }
        catch (JsonReaderException)
        {
            result = default;
            return false;
        }
    }
}