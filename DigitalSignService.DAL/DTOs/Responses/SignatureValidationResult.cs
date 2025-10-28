namespace DigitalSignService.DAL.DTOs.Responses
{
    /// <summary>
    /// Holds the validation result for a single digital signature in a PDF document.
    /// </summary>
    public class SignatureValidationResult
    {
        /// <summary>
        /// The name of the signature field.
        /// </summary>
        public string SignatureName { get; set; }

        /// <summary>
        /// Indicates if the signature's certificate was valid at the time of signing.
        /// </summary>
        public bool IsSignatureValid { get; set; }

        /// <summary>
        /// Indicates if the document has been tampered with since this signature was applied.
        /// </summary>
        public bool IsDocumentIntact { get; set; }

        /// <summary>
        /// A list of issues found during validation.
        /// </summary>
        public List<string> ValidationIssues { get; set; } = new List<string>();

        /// <summary>
        /// Detailed information about the signer.
        /// </summary>
        public SignerInfo SignerDetails { get; set; }

        /// <summary>
        /// The position and size of the signature appearance on the page.
        /// </summary>
        public SignaturePositionInfo Position { get; set; }

        /// <summary>
        /// The date and time the document was signed.
        /// </summary>
        public DateTime? SigningTime { get; set; }

        /// <summary>
        /// The reason for signing, as stated by the signer.
        /// </summary>
        public string SigningReason { get; set; }

        /// <summary>
        /// The location of signing, as stated by the signer.
        /// </summary>
        public string SigningLocation { get; set; }
    }

    /// <summary>
    /// Contains details extracted from the signer's certificate.
    /// </summary>
    public class SignerInfo
    {
        public string CommonName { get; set; }
        public string Organization { get; set; }
        public string OrganizationalUnit { get; set; }
        public string Country { get; set; }
    }

    /// <summary>
    /// Represents the location and dimensions of the signature widget.
    /// </summary>
    public class SignaturePositionInfo
    {
        public int PageNumber { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
    }
}
