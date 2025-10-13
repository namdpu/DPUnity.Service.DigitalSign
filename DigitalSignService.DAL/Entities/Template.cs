using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DigitalSignService.DAL.Entities
{
    /// <summary>
    /// Template
    /// </summary>
    [Serializable]
    [DataContract]
    [Table("template", Schema = "public")]
    public class Template : AuditEntity
    {
        /// <summary>
        /// Template name
        /// </summary>
        [DataMember]
        [Column("name")]
        [MaxLength(1024)]
        [Required]
        public string Name { get; set; }

        public virtual ICollection<TemplatePaper> TemplatePapers { get; set; }
    }
}
