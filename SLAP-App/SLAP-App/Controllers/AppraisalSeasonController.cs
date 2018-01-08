using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using SLAP_App.Models;
using SLAP_Data;
using SLAP_App.Mapper;
using System;
using System.Collections.Generic;
using SLAP_App.Services;
using System.Threading.Tasks;

namespace SLAP_App.Controllers
{
    [Authorize]
    public class AppraisalSeasonController : Controller
    {
        private AppraisalSeasonDA _appraisalSeasonDa;
        private PCAssociatesDA _pcAssociateDa;
		private ActiveDirectory _activeDirectory;

		public AppraisalSeasonController()
        {
            _appraisalSeasonDa = new AppraisalSeasonDA();
			_pcAssociateDa = new PCAssociatesDA();
            _activeDirectory=new ActiveDirectory();
		}

		// GET: AppraisalProcesses
		public ActionResult Index()
        {
            var appraisalProcessViewModels = _appraisalSeasonDa.GetAppraisalSeasons()
                .Select(x => AutoMapper.Mapper.Map<AppraisalSeasonViewModel>(x));
            return View(appraisalProcessViewModels);
        }

//        // GET: AppraisalProcesses/Details/5
//        public ActionResult Details(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            AppraisalProcess appraisalProcess = db.AppraisalProcesses.Find(id);
//            if (appraisalProcess == null)
//            {
//                return HttpNotFound();
//            }
//            return View(appraisalProcess);
//        }

        // GET: AppraisalProcesses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AppraisalProcesses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
//        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AppraisalSeasonId,Name,IsActive,PeerListFinalizationByDate,SendPeerFeedbackRequestByDate,SendPeerFeedbackByDate,SelfAppraisalSubmissionByDate,AppraisalMeetingByDate,GoalSettingByDate")] AppraisalSeasonViewModel appraisalProcessViewModel)
        {
            if (ModelState.IsValid)
            {
				try
				{
					_appraisalSeasonDa.CreateAppraisalSeason(AutoMapper.Mapper.Map<AppraisalSeason>(appraisalProcessViewModel));
					return RedirectToAction("Index", "Home");
				}
				catch (Exception ex)
				{
					ViewBag.ErrorMessage = ex.InnerException.InnerException.Message;
				}

			}

            return View(appraisalProcessViewModel);
        }

        // GET: AppraisalProcesses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppraisalSeason appraisalProcess = _appraisalSeasonDa.GetAppraisalSeason(id);
            if (appraisalProcess == null)
            {
                return HttpNotFound();
            }
            return View(AutoMapper.Mapper.Map<AppraisalSeason,AppraisalSeasonViewModel>(appraisalProcess));
        }

        // POST: AppraisalProcesses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
//        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AppraisalSeasonId,Name,IsActive,PeerListFinalizationByDate,SendPeerFeedbackRequestByDate,SendPeerFeedbackByDate,SelfAppraisalSubmissionByDate,AppraisalMeetingByDate,GoalSettingByDate")] AppraisalSeasonViewModel appraisalProcessViewModel)
        {
            if (ModelState.IsValid)
            {
				try
				{
					_appraisalSeasonDa.EditAppraisalSeason(AutoMapper.Mapper.Map<AppraisalSeasonViewModel, AppraisalSeason>(appraisalProcessViewModel));
					return RedirectToAction("Index", "Home");
				}
				catch (Exception ex)
				{
					ViewBag.ErrorMessage = ex.InnerException.InnerException.Message;
				}

			}
            return View(appraisalProcessViewModel);
        }

        // GET: AppraisalProcesses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppraisalSeason appraisalProcess = _appraisalSeasonDa.GetAppraisalSeason(id);
            if (appraisalProcess == null)
            {
                return HttpNotFound();
            }
            return View(AutoMapper.Mapper.Map<AppraisalSeason, AppraisalSeasonViewModel>(appraisalProcess));
        }
        // POST: AppraisalProcesses/Delete/5
        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _appraisalSeasonDa.DeleteAppraisalSeason(id);
            return RedirectToAction("Index", "Home");
        }

		// GET: AppraisalProcesses/Start/5
		public async Task<ActionResult> View(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			AppraisalSeason appraisalProcess = _appraisalSeasonDa.GetAppraisalSeason(id);
			if (appraisalProcess == null)
			{
				return HttpNotFound();
			}

			List<PCAssociateViewModel> pcAssociateViewModels = _pcAssociateDa.GetAllPcAssociatesForInProgressAppraisalSeason().Select(x => AutoMapper.Mapper.Map<PCAssociate, PCAssociateViewModel>(x)).ToList();
			var users = await _activeDirectory.GetAllAdUsers();
			pcAssociateViewModels.ForEach(x => {
				x.AssociateDisplayName = users.First(p => p.id == x.AssociateUserId).displayName;
				x.PCDisplayName = users.First(p => p.id == x.PCUserId).displayName;
			});
			ViewBag.PcAssociateViewModels = pcAssociateViewModels;

			return View(AutoMapper.Mapper.Map<AppraisalSeason, AppraisalSeasonViewModel>(appraisalProcess));
		}

		// GET: AppraisalProcesses/Start/5
		public async Task<ActionResult> Start(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			AppraisalSeason appraisalProcess = _appraisalSeasonDa.GetAppraisalSeason(id);
			if (appraisalProcess == null)
			{
				return HttpNotFound();
			}

			List<PCAssociateViewModel> pcAssociateViewModels = _pcAssociateDa.GetAllPcAssociatesForInProgressAppraisalSeason().Select(x => AutoMapper.Mapper.Map<PCAssociate, PCAssociateViewModel>(x)).ToList();
			var users = await _activeDirectory.GetAllAdUsers();
			pcAssociateViewModels.ForEach(x => {
				x.AssociateDisplayName = users.First(p => p.id == x.AssociateUserId).displayName;
				x.PCDisplayName = users.First(p => p.id == x.PCUserId).displayName;
			});
			ViewBag.PcAssociateViewModels = pcAssociateViewModels;

			return View(AutoMapper.Mapper.Map<AppraisalSeason, AppraisalSeasonViewModel>(appraisalProcess));
		}

		// POST: AppraisalProcesses/Delete/5
		[HttpPost, ActionName("Start")]
		//        [ValidateAntiForgeryToken]
		public ActionResult StartConfirmed(int id)
		{
			_appraisalSeasonDa.StartAppraisalSeason(id);
			return RedirectToAction("Index", "Home");
		}

		// GET: AppraisalProcesses/Complete/5
		public ActionResult Complete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			_appraisalSeasonDa.CompleteAppraisalSeason(id);
			return RedirectToAction("Index", "Home");
		}

		/*  protected override void Dispose(bool disposing)
		  {
			  if (disposing)
			  {
				  db.Dispose();
			  }
			  base.Dispose(disposing);
		  }*/
	}
}
