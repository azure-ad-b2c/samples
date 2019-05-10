using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AADB2C.CustomMFA.WebApi.Models
{
    public class InputClaimsModel
    {
        public string phoneNumber { get; set; }
        public string verificationCode { get; set; }
        public string userVerificationCode { get; set; }

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
