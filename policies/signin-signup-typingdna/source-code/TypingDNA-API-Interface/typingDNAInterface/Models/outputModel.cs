using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace typingDNAInterface.Models
{
    public class outputModel
    {
            
        public int netscore { get; set; }
        public int status { get; set; }
        public string userMessage { get; set; }
        public string version { get; set; }
        public int success { get; set; }
        public int count { get; set; }
        public bool saveTypingPattern { get; set; }
        public bool promptMFA { get; set; }
        public outputModel(string message, HttpStatusCode status)
        {
            this.userMessage = message;
            this.status = (int)status;
            this.version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}

