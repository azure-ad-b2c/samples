using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AADB2C.MFA.AuthyApp.Models
{
    public class AppSettingsModel
    {
        public string Key { get; set; }
        public string Message { get; set; }
        public int Timeout { get; set; }
        public string Logo { get; set; }
    }
}
