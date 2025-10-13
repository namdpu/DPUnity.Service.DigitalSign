using DigitalSignService.DAL.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using static DigitalSignService.Common.Enums;

namespace DigitalSignService.DAL.Entities
{
    /// <summary>
    /// History Sign
    /// </summary>
    [Serializable]
    [DataContract]
    [Table("history_sign", Schema = "public")]
    public class HistorySign : AuditEntity
    {
        /// <summary>
        /// Document url
        /// </summary>
        [DataMember]
        [Column("document_url")]
        public string DocumentUrl { get; set; }

        /// <summary>
        /// Document id
        /// </summary>
        [DataMember]
        [Column("document_id")]
        public string DocumentId { get; set; }

        /// <summary>
        /// Document name
        /// </summary>
        [DataMember]
        [Column("document_name")]
        public string DocumentName { get; set; }

        /// <summary>
        /// Template Id
        /// </summary>
        [DataMember]
        [Column("template_id")]
        public Guid? TemplateId { get; set; }

        /// <summary>
        /// User Id
        /// </summary>
        [DataMember]
        [Column("user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// User Sign Img
        /// </summary>
        [DataMember]
        [Column("img")]
        public string Img { get; set; }

        /// <summary>
        /// Reason
        /// </summary>
        [DataMember]
        [Column("reason")]
        public string Reason { get; set; }

        /// <summary>
        /// Serial Number
        /// </summary>
        [DataMember]
        [Column("serial_number")]

        public string? SerialNumber { get; set; }

        /// <summary>
        /// User Sign Positions
        /// </summary>
        [DataMember]
        [Column("user_sign_positions")]
        public UserSignPos[] UserSignPositions { get; set; }

        /// <summary>
        /// Signing Status
        /// </summary>
        [DataMember]
        [Column("signing_status")]
        public SigningStatus SigningStatus { get; set; }

        /// <summary>
        /// Document url
        /// </summary>
        [DataMember]
        [Column("document_signed_url")]
        public string? DocumentSignedUrl { get; set; }

        /// <summary>
        /// Transaction Id
        /// </summary>
        [DataMember]
        [Column("transaction_id")]
        public string? TransactionId { get; set; }

        /// <summary>
        /// Provider sign (viettel, vnpt, fpt)
        /// </summary>
        [DataMember]
        [Column("provider")]
        public string Provider { get; set; }

        public virtual Template? Template { get; set; }
    }
}
