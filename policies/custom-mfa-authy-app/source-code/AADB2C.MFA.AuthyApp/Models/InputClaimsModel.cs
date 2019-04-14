using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AADB2C.MFA.AuthyApp.Models
{
    public class InputClaimsModel
    {
        public string signInName { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public string authyId { get; set; }

        public string GetEamilAddress()
        {
            // Returns the signInName for local account, or email for social account
            return string.IsNullOrEmpty(email) == false ? email : signInName;
        }

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
