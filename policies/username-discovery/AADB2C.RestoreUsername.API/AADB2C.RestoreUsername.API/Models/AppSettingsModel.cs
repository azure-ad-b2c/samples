using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AADB2C.RestoreUsername.API.Models
{
    public class AppSettingsModel
    {
        public string SMTPServer { get; set; }
        public int SMTPPort { get; set; }
        public string SMTPUsername { get; set; }
        public string SMTPPassword { get; set; }
        public bool SMTPUseSSL { get; set; }
        public string SMTPFromAddress { get; set; }
    }
}
