namespace DogLiveBot.Data.Models;

public class SettingsMessageTextDto
{
     /// <summary>
     /// Имя пользователя
     /// </summary>
     public string UserName { get; set; }

     /// <summary>
     /// Клички собак пользователя
     /// </summary>
     public ICollection<string> DogNames { get; set; }
}