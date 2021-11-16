namespace AADMagicLinks.Models
{
    public class AppSettingsModel
    {
        public string B2CClientId { get; set; }
        public string B2CRedirectUri { get; set; }
        public string SMTPServer { get; set; }
        public int SMTPPort { get; set; }
        public string SMTPUsername { get; set; }
        public string SMTPPassword { get; set; }
        public bool SMTPUseSSL { get; set; }
        public string SMTPFromAddress { get; set; }
        public string SMTPSubject { get; set; }
        public int LinkExpiresAfterMinutes { get; set; }
        public string CertificateName { get; set; }
        public string FhirProxyUrl { get; set; }
    }
}
