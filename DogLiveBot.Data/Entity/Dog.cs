namespace DogLiveBot.Data.Entity;

public class Dog : BaseEntity<Guid>
{
    public Guid UserId { get; set; }
    public string DogName { get; set; }
}