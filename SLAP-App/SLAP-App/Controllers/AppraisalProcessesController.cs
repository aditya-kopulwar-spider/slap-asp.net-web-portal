using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SLAP_Data;

namespace SLAP_App.Controllers
{
    public class AppraisalProcessesController : Controller
    {
        private slap_dbEntities db = new slap_dbEntities();

        // GET: AppraisalProcesses
        public ActionResult Index()
        {
            return View(db.AppraisalProcesses.ToList());
        }

        // GET: AppraisalProcesses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppraisalProcess appraisalProcess = db.AppraisalProcesses.Find(id);
            if (appraisalProcess == null)
            {
                return HttpNotFound();
            }
            return View(appraisalProcess);
        }

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
        public ActionResult Create([Bind(Include = "AppraisalProcessId,AppraisalProcessYear,IsActive,PeerListFinalizationByDate,SendPeerFeedbackRequestByDate,SendPeerFeedbackByDate,SelfAppraisalSubmissionByDate,AppraisalMeetingByDate,GoalSettingByDate")] AppraisalProcess appraisalProcess)
        {
            if (ModelState.IsValid)
            {
                db.AppraisalProcesses.Add(appraisalProcess);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(appraisalProcess);
        }

        // GET: AppraisalProcesses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppraisalProcess appraisalProcess = db.AppraisalProcesses.Find(id);
            if (appraisalProcess == null)
            {
                return HttpNotFound();
            }
            return View(appraisalProcess);
        }

        // POST: AppraisalProcesses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
//        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AppraisalProcessId,AppraisalProcessYear,IsActive,PeerListFinalizationByDate,SendPeerFeedbackRequestByDate,SendPeerFeedbackByDate,SelfAppraisalSubmissionByDate,AppraisalMeetingByDate,GoalSettingByDate")] AppraisalProcess appraisalProcess)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appraisalProcess).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(appraisalProcess);
        }

        // GET: AppraisalProcesses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppraisalProcess appraisalProcess = db.AppraisalProcesses.Find(id);
            if (appraisalProcess == null)
            {
                return HttpNotFound();
            }
            return View(appraisalProcess);
        }

        // POST: AppraisalProcesses/Delete/5
        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AppraisalProcess appraisalProcess = db.AppraisalProcesses.Find(id);
            db.AppraisalProcesses.Remove(appraisalProcess);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
