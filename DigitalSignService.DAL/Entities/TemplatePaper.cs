using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DigitalSignService.DAL.Entities
{
    /// <summary>
    /// Template Paper
    /// </summary>
    [Serializable]
    [DataContract]
    [Table("template_paper", Schema = "public")]
    public class TemplatePaper : AuditEntity
    {
        /// <summary>
        /// Template Id
        /// </summary>
        [DataMember]
        [Column("template_id")]
        [Required]
        public Guid TemplateId { get; set; }

        /// <summary>
        /// Paper Size Id
        /// </summary>
        [DataMember]
        [Column("paper_size_id")]
        [Required]
        public Guid PaperSizeId { get; set; }

        public virtual Template Template { get; set; }

        public virtual PaperSize PaperSize { get; set; }

        public virtual ICollection<TemplatePaperUserSign> TemplatePaperUserSigns { get; set; }
    }
}
