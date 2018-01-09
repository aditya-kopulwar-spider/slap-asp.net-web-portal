using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLAP_App.Models
{
    public class PCAssociateViewModels
    {
        public List<PCAssociateViewModel> PcAssociateViewModels { get; set; }

        public PCAssociateViewModels()
        {
            PcAssociateViewModels = new List<PCAssociateViewModel>();
        }

    }

}