namespace DigitalSignService.DAL.Models
{
    public class DigitalSignSettings
    {
        public VnptSettings VNPT { get; set; }
        public ViettelSettings Viettel { get; set; }
        public SecuritySettings Security { get; set; }
    }

    public class VnptSettings
    {
        public string ApiUrl { get; set; }
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public bool UseSandbox { get; set; }
        public int TimeoutSeconds { get; set; } = 30;
        public string TrustedCertificatePath { get; set; }
    }

    public class ViettelSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ProfileId { get; set; }
    }

    public class SecuritySettings
    {
        public string EncryptionKey { get; set; }
        public string HashAlgorithm { get; set; } = "SHA256";
        public int KeyLength { get; set; } = 2048;
        public bool ValidateCertificateChain { get; set; } = true;
        public bool CheckRevocation { get; set; } = true;
        public bool RequireTimestamp { get; set; } = true;
        public List<string> TrustedRootCAs { get; set; } = new List<string>();
    }
}