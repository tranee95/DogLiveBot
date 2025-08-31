using DogLive.TelegramBot.Data.Enums;

namespace DogLive.TelegramBot.Data.Models;

public class DogDto
{
    /// <summary>
    /// Идентификатор собаки
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Кличка собаки
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Команда
    /// </summary>
    public CommandTypeEnum CommandType => CommandTypeEnum.DeleteDog;  
}