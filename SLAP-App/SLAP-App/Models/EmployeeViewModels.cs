using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLAP_App.Models
{
    public class EmployeeViewModels
    {
        public List<EmployeeViewModel> EmployeeModels { get; set; }
        public PCAssociateViewModel PcAssociateViewModel { get; set; }
        public HttpPostedFileBase SelfAppraisalDocument { get; set; }
        public EmployeeViewModels()
        {
            EmployeeModels=new List<EmployeeViewModel>();
        }

    }
}