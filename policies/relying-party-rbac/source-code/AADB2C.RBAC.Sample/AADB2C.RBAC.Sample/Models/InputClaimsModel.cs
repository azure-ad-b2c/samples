using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AADB2C.RBAC.Sample.Models
{
    public class InputClaimsModel
    {
        // Demo: User's object id in Azure AD B2C
        public string objectId { get; set; }

        // Demo: List of permitted  security groups user can sign-in.
        // Null or empty means, user any user can sign-in
        public string onlyMembersOf { get; set; }

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
