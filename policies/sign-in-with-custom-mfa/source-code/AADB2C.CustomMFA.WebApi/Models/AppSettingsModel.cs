using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AADB2C.CustomMFA.WebApi.Models
{
    public class AppSettingsModel
    {
        public string SMSFromPhoneNumber { get; set; }
        public string SMSUsername { get; set; }
        public string SMSPassword { get; set; }
        public string SMSMessage { get; set; }
    }
}
