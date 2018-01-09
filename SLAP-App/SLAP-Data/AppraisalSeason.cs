//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SLAP_Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class AppraisalSeason
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AppraisalSeason()
        {
            this.PCAssociates = new HashSet<PCAssociate>();
        }
    
        public int AppraisalSeasonId { get; set; }
        public string Name { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> PeerListFinalizationByDate { get; set; }
        public Nullable<System.DateTime> SendPeerFeedbackRequestByDate { get; set; }
        public Nullable<System.DateTime> SendPeerFeedbackByDate { get; set; }
        public Nullable<System.DateTime> SelfAppraisalSubmissionByDate { get; set; }
        public Nullable<System.DateTime> AppraisalMeetingByDate { get; set; }
        public Nullable<System.DateTime> GoalSettingByDate { get; set; }
        public bool IsCompleted { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PCAssociate> PCAssociates { get; set; }
    }
}
