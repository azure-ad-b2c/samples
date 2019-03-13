using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace Contoso.AADB2C.API.Models
{
    public class TableUser : TableEntity
    {
        public TableUser(string email)
        {
            this.PartitionKey = email;
        }

        public TableUser() { }

        public string email { get; set; }
        public Boolean migrated { get; set; }
        public string upn { get; set; }
    }
}