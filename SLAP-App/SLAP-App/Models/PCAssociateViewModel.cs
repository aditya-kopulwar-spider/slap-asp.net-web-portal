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
        public int AppraisalSeasonId { get; set; }
        public string SelfAppraisalDocumentUrl { get; set; }
        public Nullable<System.DateTime> AppraisalDate { get; set; }
        public Nullable<System.DateTime> LastNotificationDate { get; set; }
        public bool PeerListFinalized { get; set; }
        public List<PeerViewModel> Peers { get; set; }
        public HttpPostedFileBase SelfAppraisalDocument { get; set; }
        public PCAssociateViewModel()
        {
            Peers = new List<PeerViewModel>();
        }
    }
}