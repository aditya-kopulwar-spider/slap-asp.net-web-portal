using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLAP_App.Models
{
    public class AssociatePeerSelectionModel
    {
        public List<PeerAssociateViewModel> PeerAssociateViewModels { get; set; }

        public AssociatePeerSelectionModel()
        {
            this.PeerAssociateViewModels = new List<PeerAssociateViewModel>();
        }
    }
}