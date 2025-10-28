using System.ComponentModel.DataAnnotations;

namespace DigitalSignService.DAL.DTOs.Requests
{
    /// <summary>
    /// Represents the request for verifying PDF signatures, which can be provided as a file or a URL.
    /// </summary>
    public class VerifySignatureRequest : IValidatableObject
    {
        /// <summary>
        /// The URL of the PDF file to download and verify. Use this or the File, but not both.
        /// </summary>
        public string? Url { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //if (File == null && string.IsNullOrWhiteSpace(Url))
            //{
            //    yield return new ValidationResult("Either a file or a URL must be provided.", new[] { nameof(File), nameof(Url) });
            //}

            //if (File != null && !string.IsNullOrWhiteSpace(Url))
            //{
            //    yield return new ValidationResult("Cannot provide both a file and a URL.", new[] { nameof(File), nameof(Url) });
            //}

            //if (File != null && Path.GetExtension(File.FileName).ToLower() != ".pdf")
            //{
            //    yield return new ValidationResult("Only PDF files are accepted.", new[] { nameof(File) });
            //}

            if (string.IsNullOrWhiteSpace(Url))
            {
                yield return new ValidationResult("Either a URL must be provided.", new[] { nameof(Url) });
            }

            if (!string.IsNullOrWhiteSpace(Url))
            {
                if (!Uri.TryCreate(Url, UriKind.Absolute, out var uriResult) || (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
                {
                    yield return new ValidationResult("The URL provided is not a valid HTTP/HTTPS URL.", new[] { nameof(Url) });
                }
            }
        }
    }
}
