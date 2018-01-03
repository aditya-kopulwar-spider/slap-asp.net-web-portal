using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace SLAP_Data
{
 public  class PCAssociatesDA
    {
        private slap_dbEntities _dbEntities;
        AppraisalProcessDA _appraisalProcessDa ;

        public PCAssociatesDA()
        {
            _dbEntities = new slap_dbEntities();
            _appraisalProcessDa = new AppraisalProcessDA();
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
//            pcAssociate.SelfAppraisalStatus = false;
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

        public bool RemoveAllAssociatesForGivenPC(Guid pcID)
        {
            _dbEntities.PCAssociates.RemoveRange(_dbEntities.PCAssociates.Where(p => p.PCUserId == pcID).ToList());
            _dbEntities.SaveChanges();
            return true;
        }

        public List<PCAssociate> GetAllAssociateForGivenPC(Guid PcID)
        {
            return _dbEntities.PCAssociates.Where(pcAssociate => pcAssociate.PCUserId == PcID).ToList(); ;
        }

        public PCAssociate GetPCAssociate(int? pcAssociateId)
        {
            return _dbEntities.PCAssociates.Find(pcAssociateId);
        }
        public PCAssociate EditPCAssociate(PCAssociate pcAssociate)
        {
            _dbEntities.Entry(pcAssociate).State = EntityState.Modified;
            _dbEntities.SaveChanges();
            return pcAssociate;
        }

        public List<PCAssociate> GetAllCurrentYearPcAssociates()
        {
            var appraisalProcessId = _appraisalProcessDa.GetActiveAppraisalProces().AppraisalProcessId;
            return _dbEntities.PCAssociates.Where(p=>p.AppraisalProcessId== appraisalProcessId).ToList();
        }
        public bool AddAsociates(List<PCAssociate> pcAssociates)
        {
            var appraisalProcesses = _dbEntities.AppraisalProcesses.First(p => p.IsActive == true);
            pcAssociates.ForEach(p=>p.AppraisalProcessId=appraisalProcesses.AppraisalProcessId);
            _dbEntities.PCAssociates.AddRange(pcAssociates);
            _dbEntities.SaveChanges();
            return true;
        }
        public bool RemoveAsociates(List<PCAssociate> pcAssociates)
        {
//            var appraisalProcesses = _dbEntities.AppraisalProcesses.First(p => p.IsActive == true);
//            pcAssociates.ForEach(p => p.AppraisalProcessId = appraisalProcesses.AppraisalProcessId);
            _dbEntities.PCAssociates.RemoveRange(pcAssociates);
            _dbEntities.SaveChanges();
            return true;
        }
        public List<PCAssociate> GetAllCurrentYearPcAssociatesForGivenPCId(Guid pcId)
        {
            var appraisalProcessId = _appraisalProcessDa.GetActiveAppraisalProces().AppraisalProcessId;
            return _dbEntities.PCAssociates.Where(p => p.AppraisalProcessId == appraisalProcessId && p.PCUserId==pcId).ToList();
        }
    }
}
