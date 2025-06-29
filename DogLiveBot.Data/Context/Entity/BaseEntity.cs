using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogLiveBot.Data.Context.Entity
{
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
        [Column(TypeName = "timestamp without time zone")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Время последнего изменения сущности.
        /// </summary>
        [Column(TypeName = "timestamp without time zone")]
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Время удаления сущности (если применимо).
        /// </summary>
        [Column(TypeName = "timestamp without time zone")]
        public DateTime? DeleteDate { get; set; }
    }
}