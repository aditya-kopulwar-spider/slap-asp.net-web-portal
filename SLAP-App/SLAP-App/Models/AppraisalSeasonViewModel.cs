using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SLAP_App.Models
{
    public class AppraisalSeasonViewModel
    {
        public int AppraisalSeasonId { get; set; }
		[Required(AllowEmptyStrings = false)]
		public string Name { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> PeerListFinalizationByDate { get; set; }
        public Nullable<System.DateTime> SendPeerFeedbackRequestByDate { get; set; }
        public Nullable<System.DateTime> SendPeerFeedbackByDate { get; set; }
        public Nullable<System.DateTime> SelfAppraisalSubmissionByDate { get; set; }
        public Nullable<System.DateTime> AppraisalMeetingByDate { get; set; }
        public Nullable<System.DateTime> GoalSettingByDate { get; set; }

		public bool AreAllDatesSet()
		{
			return PeerListFinalizationByDate.HasValue &&
				SendPeerFeedbackRequestByDate.HasValue &&
				SendPeerFeedbackByDate.HasValue &&
				SelfAppraisalSubmissionByDate.HasValue &&
				AppraisalMeetingByDate.HasValue &&
				GoalSettingByDate.HasValue;
		}
    }
}