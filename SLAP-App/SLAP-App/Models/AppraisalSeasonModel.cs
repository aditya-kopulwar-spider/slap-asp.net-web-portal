using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLAP_App.Models
{
    public class AppraisalSeasonModel
    {
        public int AppraisalProcessId { get; set; }
        public short AppraisalProcessYear { get; set; }
        public bool IsActive { get; set; }
        public DateTime PeerListFinalizationByDate { get; set; }
        public DateTime SendPeerFeedbackRequestByDate { get; set; }
        public DateTime SendPeerFeedbackByDate { get; set; }
        public DateTime SelfAppraisalSubmissionByDate { get; set; }
        public DateTime AppraisalMeetingByDate { get; set; }
        public DateTime GoalSettingByDate { get; set; }
    }
}