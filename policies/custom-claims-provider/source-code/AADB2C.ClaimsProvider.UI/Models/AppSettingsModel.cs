using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AADB2C.ClaimsProvider.UI.Models
{
    public class AppSettingsModel
    {
        public string BlobStorageConnectionString { get; set; }
        public string CertificateName { get; set; }
        public string CertificatePassword { get; set; }
        public string EncryptionKey { get; set; }
        public string AuthorizedRedirectUri { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Tenant { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
