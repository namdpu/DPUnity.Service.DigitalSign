using DigitalSignService.DAL.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DigitalSignService.DAL.Entities
{
    /// <summary>
    /// Template Paper Sign
    /// </summary>
    [Serializable]
    [DataContract]
    [Table("template_paper_user_sign", Schema = "public")]
    public class TemplatePaperUserSign : AuditEntity
    {
        /// <summary>
        /// Template Paper Id
        /// </summary>
        [DataMember]
        [Column("template_paper_id")]
        public Guid TemplatePaperId { get; set; }

        /// <summary>
        /// User Sign Id
        /// </summary>
        [DataMember]
        [Column("user_sign_id")]
        public string UserSignId { get; set; }

        /// <summary>
        /// Priority sign in document by user
        /// </summary>
        [DataMember]
        [Column("priority")]
        public int Priority { get; set; }

        /// <summary>
        /// User Sign Img
        /// </summary>
        [DataMember]
        [Column("img")]
        public string Img { get; set; }

        /// <summary>
        /// Rotate sign image
        /// </summary>
        [DataMember]
        [Column("rotate")]
        public int? Rotate { get; set; }

        /// <summary>
        /// User Sign Positions
        /// </summary>
        [DataMember]
        [Column("user_sign_positions")]
        public UserSignPos[]? UserSignPositions { get; set; }

        public virtual TemplatePaper TemplatePaper { get; set; }
    }
}
