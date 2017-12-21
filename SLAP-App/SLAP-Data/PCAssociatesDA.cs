using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLAP_Data
{
 public  class PCAssociatesDA
    {
        private slap_dbEntities _dbEntities;

        public PCAssociatesDA()
        {
            _dbEntities = new slap_dbEntities();
        }

        public List<PCAssociate> GetAllPcAssociates()
        {
            return _dbEntities.PCAssociates.ToList();
        }

        public bool AddAsociate(PCAssociate pcAssociate)
        {
            //todo appraisal season selection from ui(multiple seasons are active) or in code(if one is active only)
            var appraisalProcesses = _dbEntities.AppraisalProcesses.First(p => p.IsActive == true);
            pcAssociate.AppraisalProcessId = appraisalProcesses.AppraisalProcessId;
            pcAssociate.SelfAppraisalStatus = false;
            _dbEntities.PCAssociates.Add(pcAssociate);
            _dbEntities.SaveChanges();
            return true;
        }

        public bool RemoveAssociate(Guid associateId, Guid pcId)
        {
            PCAssociate pcAssociate =
                _dbEntities.PCAssociates.FirstOrDefault(p => p.PCUserId == pcId && p.AssociateUserId == associateId);
            if (pcAssociate != null) _dbEntities.PCAssociates.Remove(pcAssociate);
            _dbEntities.SaveChanges();
            return true;
        }
    }
}
