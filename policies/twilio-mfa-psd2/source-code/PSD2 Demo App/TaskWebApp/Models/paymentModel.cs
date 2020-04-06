using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaskWebApp.Models
{
    public class paymentModel
    {
        [Required]
        public string Amount { get; set; }
        [Required]
        public string Payee { get; set; }
        [Required]
        public string AccountNumber { get; set; }
    }
}