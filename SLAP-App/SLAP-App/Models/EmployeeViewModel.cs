using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLAP_App.Models
{
    public class EmployeeViewModel
    {
        public int PeerAssociateId { get; set; }
        public System.Guid AssociateUserId { get; set; }
        public string AssociateName { get; set; }
        public string PeerName { get; set; }
        public System.Guid PeerUserId { get; set; }
        public int AppraisalProcessId { get; set; }
        public Nullable<System.DateTime> LastNotificationDate { get; set; }
        public bool ShareFeedbackWithAssociate { get; set; }
        public string FeedbackDocumentUrl { get; set; }
        public HttpPostedFileBase FeedbackDocument { get; set; }
    }
}