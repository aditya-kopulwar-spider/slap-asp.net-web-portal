using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using SLAP_App.Models;
using SLAP_Data;
using SLAP_App.Mapper;

namespace SLAP_App.Controllers
{
    [Authorize]
    public class AppraisalProcessesController : Controller
    {
        private AppraisalSeasonDA _appraisalProcessDa;

        public AppraisalProcessesController()
        {
            _appraisalProcessDa = new AppraisalSeasonDA();
        }

        // GET: AppraisalProcesses
        public ActionResult Index()
        {
            var appraisalProcessViewModels = _appraisalProcessDa.GetAppraisalProcesses()
                .Select(x => AutoMapper.Mapper.Map<AppraisalProcessViewModel>(x));
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
        public ActionResult Create([Bind(Include = "AppraisalProcessId,AppraisalProcessYear,IsActive,PeerListFinalizationByDate,SendPeerFeedbackRequestByDate,SendPeerFeedbackByDate,SelfAppraisalSubmissionByDate,AppraisalMeetingByDate,GoalSettingByDate")] AppraisalProcessViewModel appraisalProcessViewModel)
        {
            if (ModelState.IsValid)
            {
                _appraisalProcessDa.CreateAppraisalProcess(AutoMapper.Mapper.Map<AppraisalSeason>(appraisalProcessViewModel));
                return RedirectToAction("Index");
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
            AppraisalSeason appraisalProcess = _appraisalProcessDa.GetAppraisalProcess(id);
            if (appraisalProcess == null)
            {
                return HttpNotFound();
            }
            return View(AutoMapper.Mapper.Map<AppraisalSeason,AppraisalProcessViewModel>(appraisalProcess));
        }

        // POST: AppraisalProcesses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
//        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AppraisalProcessId,AppraisalProcessYear,IsActive,PeerListFinalizationByDate,SendPeerFeedbackRequestByDate,SendPeerFeedbackByDate,SelfAppraisalSubmissionByDate,AppraisalMeetingByDate,GoalSettingByDate")] AppraisalProcessViewModel appraisalProcessViewModel)
        {
            
            if (ModelState.IsValid)
            {
                _appraisalProcessDa.EditAppraisalProcess(AutoMapper.Mapper.Map<AppraisalProcessViewModel, AppraisalSeason>(appraisalProcessViewModel));
                return RedirectToAction("Index");
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
            AppraisalSeason appraisalProcess = _appraisalProcessDa.GetAppraisalProcess(id);
            if (appraisalProcess == null)
            {
                return HttpNotFound();
            }
            return View(AutoMapper.Mapper.Map<AppraisalSeason, AppraisalProcessViewModel>(appraisalProcess));
        }
        // POST: AppraisalProcesses/Delete/5
        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _appraisalProcessDa.DeleteAppraisalProcess(id);
            return RedirectToAction("Index");
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
