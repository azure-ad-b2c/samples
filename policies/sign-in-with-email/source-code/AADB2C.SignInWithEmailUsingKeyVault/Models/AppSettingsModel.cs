using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AADB2C.SignInWithEmailUsingKeyVault.Models
{
    public class AppSettingsModel
    {
        public string B2CTenant { get; set; }
        public string B2CPolicy { get; set; }
        public string B2CClientId { get; set; }
        public string B2CRedirectUri { get; set; }
        public string B2CSignUpUrl { get; set; }

        public string SMTPServer { get; set; }
        public int SMTPPort { get; set; }
        public string SMTPUsername { get; set; }
        public string SMTPPassword { get; set; }
        public bool SMTPUseSSL { get; set; }
        public string SMTPFromAddress { get; set; }
        public string SMTPSubject { get; set; }

        public int LinkExpiresAfterMinutes { get; set; }
        
        public string VaultUrl { get; set; }
        public string CertificateName { get; set; }
    }
}
