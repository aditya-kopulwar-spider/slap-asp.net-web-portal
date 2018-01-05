using SLAP_App.Models;
using SLAP_Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SLAP_App.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
		private AppraisalSeasonDA _appraisalSeasonDa;

		public HomeController()
		{
			_appraisalSeasonDa = new AppraisalSeasonDA();
		}

		public ActionResult Index()
        {
			AppraisalSeason activeAppraisalSeason = _appraisalSeasonDa.GetInProgressAppraisalSeason();
			ViewBag.ActiveAppraisalSeason = AutoMapper.Mapper.Map<AppraisalSeasonViewModel>(activeAppraisalSeason);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}