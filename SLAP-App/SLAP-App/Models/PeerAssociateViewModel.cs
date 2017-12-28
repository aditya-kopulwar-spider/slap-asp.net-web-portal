using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLAP_App.Models
{
    public class PeerAssociateViewModel
    {
        public String PeerDisplayName { get; set; }
        public Guid PeerId { get; set; }
        public bool IsPeer { get; set; }
        public Guid AssociateUserId { get; set; }
    }
}