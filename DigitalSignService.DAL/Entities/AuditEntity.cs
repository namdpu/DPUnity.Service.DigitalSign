using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalSignService.DAL.Entities
{
    /// <summary>The audit entity.</summary>
    [Serializable]
    public abstract class AuditEntity : BaseEntity
    {
        /// <summary>Gets or sets the created user ıd.</summary>
        [Column("created_userId")]
        public Guid? CreatedUserId { get; set; }

        /// <summary>Gets or sets the created date time.</summary>
        [Column("created_dateTime")]
        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;

        /// <summary>Gets or sets the updated user ıd.</summary>
        [Column("updated_userId")]
        public Guid? UpdatedUserId { get; set; }

        /// <summary>Gets or sets the updated date time.</summary>
        [Column("updated_dateTime")]
        public DateTime? UpdatedDateTime { get; set; }
    }
}
