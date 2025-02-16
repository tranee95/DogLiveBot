namespace DogLiveBot.Data.Entity;

public class User : BaseEntity<Guid>
{
    public string Fio { get; set; }

    public ICollection<Dog> Dogs { get; set; }
}