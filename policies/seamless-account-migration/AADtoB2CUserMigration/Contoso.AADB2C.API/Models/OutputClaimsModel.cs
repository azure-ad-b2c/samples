using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contoso.AADB2C.API.Models
{
    public class OutputClaimsModel
    {
        public bool tokenSuccess { get; set; }
        public bool migrationRequired { get; set; }
    }
}