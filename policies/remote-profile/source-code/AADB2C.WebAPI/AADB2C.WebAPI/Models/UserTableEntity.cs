using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AADB2C.WebAPI.Models
{
    public class UserTableEntity: TableEntity
    {
        public UserTableEntity()
        {

        }

        public UserTableEntity(string objectId, string email, string displayName, string givenName, string surName)
        {
            this.PartitionKey = Consts.MigrationTablePartition;
            this.RowKey = objectId.ToLower();
            this.Email = email;
            this.DisplayName = displayName;
            this.GivenName = givenName;
            this.SurName = surName;
        }

        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string GivenName { get; set; }
        public string SurName { get; set; }
    }
}
