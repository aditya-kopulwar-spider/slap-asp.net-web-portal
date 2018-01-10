using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SLAP_App.Models
{
    public class User
    {
        public System.Guid id { get; set; }
        public List<object> businessPhones { get; set; }
        public string displayName { get; set; }
        public string givenName { get; set; }
        public object jobTitle { get; set; }
        public string mail { get; set; }
        public object mobilePhone { get; set; }
        public object officeLocation { get; set; }
        public string preferredLanguage { get; set; }
        public string surname { get; set; }
        public string userPrincipalName { get; set; }
        public bool IsPC { get; set; }
        public bool IsAdmin { get; set; }

		public User PC { get; set; }
		public PCAssociateViewModel PCAssociateModel { get; set; }
		public IList<PeerViewModel> SeekingFeedbackFrom { get; set; }
		public IList<PeerViewModel> SendingFeedbackTo { get; set; }
	}

    public class RootObject
    {
        public string context { get; set; }
        [JsonProperty("Value")]
        public List<User> Users { get; set; }
    }
}