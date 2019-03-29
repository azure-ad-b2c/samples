using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AADB2C.ClaimsProvider.API.Models
{
    public class AppSettingsModel
    {
        public string BlobStorageConnectionString { get; set; }
        public string EncryptionKey { get; set; }
    }
}
