using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLAP_Data
{
    public class AppraisalSeasonDA
    {
        private slap_dbEntities _dbEntities;

        public AppraisalSeasonDA()
        {
            _dbEntities = new slap_dbEntities();
        }

        public List<AppraisalSeason> GetAppraisalProcesses()
        {
            return _dbEntities.AppraisalSeasons.ToList();
        }

        public bool CreateAppraisalProcess(AppraisalSeason appraisalProcess)
        {
            _dbEntities.AppraisalSeasons.Add(appraisalProcess);
            return _dbEntities.SaveChanges() > 0;
        }

        public AppraisalSeason EditAppraisalProcess(AppraisalSeason appraisalProcess)
        {
            _dbEntities.Entry(appraisalProcess).State = EntityState.Modified;
            _dbEntities.SaveChanges();
            return appraisalProcess;
        }

        public bool DeleteAppraisalProcess(int appraisalProcessId)
        {
            AppraisalSeason appraisalSeason = _dbEntities.AppraisalSeasons.Find(appraisalProcessId);
            _dbEntities.AppraisalSeasons.Remove(appraisalSeason);
            _dbEntities.SaveChanges();
            return true;
        }

        public AppraisalSeason GetAppraisalProcess(int? id)
        {
            return _dbEntities.AppraisalSeasons.Find(id);
        }

        public AppraisalSeason GetActiveAppraisalProces()
        {
            return _dbEntities.AppraisalSeasons.FirstOrDefault(p => p.IsActive == true);
        }
    }
}