using DigitalSignService.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DigitalSignService.DAL.Entities
{
    /// <summary>
    /// Paper Size
    /// </summary>
    [Serializable]
    [DataContract]
    [Table("paper_size", Schema = "public")]
    public class PaperSize : AuditEntity
    {
        /// <summary>
        /// Paper size name
        /// </summary>
        [DataMember]
        [Column("paper_name")]
        [Required]
        [MaxLength(1024)]
        public string PaperName { get; set; }

        /// <summary>
        /// Paper size height in mm
        /// </summary>
        [DataMember]
        [Column("height")]
        public int Height { get; set; }

        /// <summary>
        /// Paper size width in mm
        /// </summary>
        [DataMember]
        [Column("width")]
        public int Width { get; set; }

        /// <summary>
        /// paper size type
        /// </summary>
        [DataMember]
        [Column("paper_size_type")]
        [Required]
        public Enums.PaperSizeType PaperSizeType { get; set; }

        public virtual ICollection<TemplatePaper> TemplatePapers { get; set; }
    }
}
