using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogLiveBot.Data.Entity;

public class BaseEntity<T>
{
    /// <summary>
    /// Идентификатор сущности.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public T Id { get; set; }

    /// <summary>
    /// Время создания сущности.
    /// </summary>
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// Время последнего изменения сущности.
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    /// <summary>
    /// Время удаления сущности (если применимо).
    /// </summary>
    public DateTime? DeleteDate { get; set; }
}