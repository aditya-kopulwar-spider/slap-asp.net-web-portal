using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLAP_App.Models
{
    public class PCAssociateViewModel
    {
        public string PCDisplayName { get; set; }
        public string AssociateDisplayName { get; set; }
		public int PCAssociatesId { get; set; }
        public Guid PCUserId { get; set; }
        public Guid AssociateUserId { get; set; }
        public int AppraisalProcessId { get; set; }
        public bool SelfAppraisalStatus { get; set; }
        public Nullable<System.DateTime> AppraisalDate { get; set; }
        public Nullable<System.DateTime> LastNotificationDate { get; set; }
    }
}