using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DigitalSignService.DAL.Entities
{
    /// <summary>The base entity.</summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class BaseEntity
    {
        /// <summary>Gets or sets the ıd.</summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DataMember]
        [Column("id")]
        public Guid Id { get; set; }

        /// <summary>Gets or sets a value indicating whether ıs active.</summary>
        [DataMember]
        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        /// <summary>Gets or sets a value indicating whether ıs deleted.</summary>
        [DataMember]
        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;
    }
}
