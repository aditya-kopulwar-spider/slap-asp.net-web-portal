using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLAP_Data
{
    public class AppraisalProcessDA
    {
        private slap_dbEntities _dbEntities;

        public AppraisalProcessDA()
        {
            _dbEntities = new slap_dbEntities();
        }

        public List<AppraisalProcess> GetAppraisalProcesses()
        {
            return _dbEntities.AppraisalProcesses.ToList();
        }

        public bool CreateAppraisalProcess(AppraisalProcess appraisalProcess)
        {
            _dbEntities.AppraisalProcesses.Add(appraisalProcess);
            return _dbEntities.SaveChanges() > 0;
        }

        public AppraisalProcess EditAppraisalProcess(AppraisalProcess appraisalProcess)
        {
            _dbEntities.Entry(appraisalProcess).State = EntityState.Modified;
            _dbEntities.SaveChanges();
            return appraisalProcess;
        }

        public bool DeleteAppraisalProcess(int appraisalProcessId)
        {
            AppraisalProcess appraisalProcess = _dbEntities.AppraisalProcesses.Find(appraisalProcessId);
            _dbEntities.AppraisalProcesses.Remove(appraisalProcess);
            _dbEntities.SaveChanges();
            return true;
        }

    }
}