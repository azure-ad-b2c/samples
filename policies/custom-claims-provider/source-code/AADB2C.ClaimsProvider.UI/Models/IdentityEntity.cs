using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AADB2C.ClaimsProvider.UI.Models
{
    public class IdentityEntity : TableEntity
    {
        public IdentityEntity(string partitionKey, string rowKey)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
        }

        public IdentityEntity() { }

        public string ipAddress { get; set; }

        public string userId { get; set; }
        public string appId { get; set; }

        public string City { get; set; }
    }
}
