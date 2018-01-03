using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLAP_App.Models
{
    public class AssociateSelectionViewModel
    {
        public IList<PCAssociateUserViewModel> PcAssociateUserViewModels { get; set; }

      public  AssociateSelectionViewModel()
        {
            this.PcAssociateUserViewModels=new List<PCAssociateUserViewModel>();
        }

        public IEnumerable<Guid> getSelectedAssociates()
        {
            return PcAssociateUserViewModels.Where(p => p.Selected == true).Select(p => p.AssociateUserId);
        }
    }
    
}