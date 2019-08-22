using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AADB2C.MFA.TOTP.Models
{
    public class AppSettingsModel
    {
        public string TOTPIssuer { get; set; }
        public string TOTPAccountPrefix { get; set; }
        public int TOTPTimestep { get; set; }
        public string EncryptionKey { get; set; }
    }
}
