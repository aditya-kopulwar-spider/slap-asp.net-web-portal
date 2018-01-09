using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLAP_App.Models
{
    public class PeerViewModel
    {
        public int PeerAssociateId { get; set; }
        public System.Guid AssociateUserId { get; set; }
        public System.Guid PeerUserId { get; set; }
        public string PeerName { get; set; }
        public int AppraisalProcessId { get; set; }
        public string FeedbackDocumentUrl { get; set; }
        public Nullable<System.DateTime> LastNotificationDate { get; set; }
        public int PCAssociateId { get; set; }
    }
}