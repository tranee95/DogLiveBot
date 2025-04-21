using DogLiveBot.Data.Enums;

namespace DogLiveBot.Data.Model;

public class CommandDataModel
{
    /// <summary>
    /// Идентификатор сущности
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Тип команды 
    /// </summary>
    public CommandTypeEnum CommandType { get; set; }
}