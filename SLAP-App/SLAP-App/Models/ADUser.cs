using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLAP_App.Models
{
    public class ADUser
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsPC { get; set; }
    }
}