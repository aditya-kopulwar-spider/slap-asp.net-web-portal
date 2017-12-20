using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLAP_App.Models
{
    public class Peer
    {
        public int PeerAssociateId { get; set; }
        public System.Guid AssociateUserId { get; set; }
        public System.Guid PeerUserId { get; set; }
        public int AppraisalProcessId { get; set; }
        public bool FeedbackStatus { get; set; }
        public System.DateTime LastNotificationDate { get; set; }
    }
}