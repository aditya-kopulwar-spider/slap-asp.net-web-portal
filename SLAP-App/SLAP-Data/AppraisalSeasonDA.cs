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

        public List<AppraisalSeason> GetAppraisalSeasons()
        {
            return _dbEntities.AppraisalSeasons.ToList();
        }

        public bool CreateAppraisalSeason(AppraisalSeason appraisalSeason)
        {
            _dbEntities.AppraisalSeasons.Add(appraisalSeason);
            return _dbEntities.SaveChanges() > 0;
        }

        public AppraisalSeason EditAppraisalSeason(AppraisalSeason appraisalSeason)
        {
            _dbEntities.Entry(appraisalSeason).State = EntityState.Modified;
            _dbEntities.SaveChanges();
            return appraisalSeason;
        }

        public bool DeleteAppraisalSeason(int appraisalSeasonId)
        {
            AppraisalSeason appraisalSeason = _dbEntities.AppraisalSeasons.Find(appraisalSeasonId);
            _dbEntities.AppraisalSeasons.Remove(appraisalSeason);
            _dbEntities.SaveChanges();
            return true;
        }

        public AppraisalSeason GetAppraisalSeason(int? id)
        {
            return _dbEntities.AppraisalSeasons.Find(id);
        }

		public AppraisalSeason GetActiveAppraisalSeason()
		{
			return _dbEntities.AppraisalSeasons.FirstOrDefault(p => p.IsActive == true);
		}

		public AppraisalSeason GetInProgressAppraisalSeason()
		{
			return _dbEntities.AppraisalSeasons.FirstOrDefault(p => p.IsCompleted == false);
		}

		public void StartAppraisalSeason(int? id)
		{
			AppraisalSeason appraisalSeason = GetAppraisalSeason(id);
			appraisalSeason.IsActive = true;
			_dbEntities.Entry(appraisalSeason).State = EntityState.Modified;
			_dbEntities.SaveChanges();
		}

		public void CompleteAppraisalSeason(int? id)
		{
			AppraisalSeason appraisalSeason = GetAppraisalSeason(id);
			appraisalSeason.IsActive = false;
			appraisalSeason.IsCompleted = true;
			_dbEntities.Entry(appraisalSeason).State = EntityState.Modified;
			_dbEntities.SaveChanges();
		}
	}
}