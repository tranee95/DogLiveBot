using System.Text.Json;
using System.Text.Json.Serialization;
using DogLive.TelegramBot.Data.Enums;

namespace DogLive.TelegramBot.Data.Models.CommandData;

public class CommandData
{
    public CommandData()
    {
    }

    public CommandData(CommandTypeEnum commandType, object data)
    {
        CommandType = commandType;

        _options = new JsonSerializerOptions
        {
            WriteIndented = false,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        Data = JsonSerializer.Serialize(data, _options);
    }

    private JsonSerializerOptions _options { get; }

    /// <summary>
    /// Данные команды
    /// </summary>
    [JsonPropertyName("d")]
    public string Data { get; set; }

    /// <summary>
    /// Тип команды 
    /// </summary>
    [JsonPropertyName("c")]
    public CommandTypeEnum CommandType { get; set; }

    // Получение данных
    public T GetData<T>() 
    {
        return JsonSerializer.Deserialize<T>(Data, _options);
    }
}