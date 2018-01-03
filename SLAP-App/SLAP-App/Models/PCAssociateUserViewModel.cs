using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLAP_App.Models
{
    public class PCAssociateUserViewModel
    {
        public bool Selected { get; set; }
        public String AssociateDisplayName { get; set; }
        public System.Guid PCUserId { get; set; }
        public System.Guid AssociateUserId { get; set; }
    }
}