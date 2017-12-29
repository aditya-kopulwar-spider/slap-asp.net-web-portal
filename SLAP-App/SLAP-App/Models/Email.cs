using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLAP_App.Models
{
    [Serializable]
    public class Email
    {
        public string ToEmail { get; set; }
        public string FromEmail { get; set; }
        public string CcEmail { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}