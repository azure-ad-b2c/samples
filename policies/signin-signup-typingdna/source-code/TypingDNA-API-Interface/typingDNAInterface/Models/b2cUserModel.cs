using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace typingDNAInterface.Models
{
    public class b2cUserModel
    {
        public string objectid { get; set; }
        public string typingPattern { get; set; }
        public int count { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static b2cUserModel Parse(string JSON)
        {
            return JsonConvert.DeserializeObject(JSON, typeof(b2cUserModel)) as b2cUserModel;
        }
    }
}
