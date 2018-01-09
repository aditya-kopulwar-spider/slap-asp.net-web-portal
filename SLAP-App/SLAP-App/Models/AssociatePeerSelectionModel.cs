using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLAP_App.Models
{
    public class AssociatePeerSelectionModel
    {
        public List<PeerViewModel> PeerViewModels { get; set; }
        public bool PeerListFinalized { get; set; }

        public AssociatePeerSelectionModel()
        {
            this.PeerViewModels = new List<PeerViewModel>();
        }
    }
}