using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AADB2C.RBAC.Sample.Models
{
    public class GraphGroupsModel
    {
        public string odatametadata { get; set; }
        public List<GraphGroupModel> value { get; set; }

        public static GraphGroupsModel Parse(string JSON)
        {
            return JsonConvert.DeserializeObject(JSON.Replace("odata.metadata", "odatametadata"), typeof(GraphGroupsModel)) as GraphGroupsModel;
        }
    }

    public class GraphGroupModel
    {
        public string objectType { get; set; }
        public string objectId { get; set; }
        public object deletionTimestamp { get; set; }
        public string description { get; set; }
        public object dirSyncEnabled { get; set; }
        public string displayName { get; set; }
        public object lastDirSyncTime { get; set; }
        public object mail { get; set; }
        public string mailNickname { get; set; }
        public bool mailEnabled { get; set; }
        public bool securityEnabled { get; set; }
    }
}
