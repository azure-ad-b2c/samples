using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AADB2C.ClaimsProvider.API.Models
{
    public class InputClaimsModel
    {
        public string correlationId { get; set; }
        public string userId { get; set; }
        public string ipAddress { get; set; }
        public string appId { get; set; }

        //Custom attributes
        public string city { get; set; }

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
