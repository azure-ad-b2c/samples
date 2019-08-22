using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AADB2C.MFA.TOTP.Models
{
    public class InputClaimsModel
    {
        public string userName { get; set; }
        public string secretKey { get; set; }
        public string totpCode { get; set; }
        public string timeStepMatched { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static InputClaimsModel Parse(string JSON)
        {
            return JsonConvert.DeserializeObject(JSON, typeof(InputClaimsModel)) as InputClaimsModel;
        }
    }
}
