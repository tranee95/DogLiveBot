using DogLiveBot.Data.Enums;

namespace DogLiveBot.Data.Models;

public class DogDeleteModel
{
    /// <summary>
    /// Идентификатор собаки
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Кличка собаки
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Команда
    /// </summary>
    public CommandTypeEnum CommandType => CommandTypeEnum.DeleteDog;  
}