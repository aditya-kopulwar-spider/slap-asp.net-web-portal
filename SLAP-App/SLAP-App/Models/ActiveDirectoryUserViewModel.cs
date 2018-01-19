using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLAP_App.Models
{
    public class ActiveDirectoryUserViewModel
    {
        public int ActiveDirectoryUserId { get; set; }
        public System.Guid Id { get; set; }
        public string userPrincipalName { get; set; }
        public string givenName { get; set; }
        public string displayName { get; set; }
        public Nullable<int> ManagerId { get; set; }
        public  ICollection<ActiveDirectoryUserViewModel> Associates { get; set; }
        public  ActiveDirectoryUserViewModel Manager { get; set; }
    }
}