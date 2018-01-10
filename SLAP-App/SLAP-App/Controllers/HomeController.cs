using SLAP_App.Models;
using SLAP_App.Services;
using SLAP_Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SLAP_App.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
		public const string SK_CURRENT_USER = "CurrentUser";
		private AppraisalSeasonDA _appraisalSeasonDa;
		private ActiveDirectory _activeDirectory;
		private UserRolesDA _userRolesDA = new UserRolesDA();

		public HomeController()
		{
			_appraisalSeasonDa = new AppraisalSeasonDA();
			_activeDirectory = new ActiveDirectory();
		}

		public async Task<ActionResult> Index()
        {
			User loggedInUser = null;
			if (Session[SK_CURRENT_USER] == null)
			{
				var identityName = User.Identity.Name;
				//var users = await _activeDirectory.GetAllAdUsers();
				loggedInUser = await _activeDirectory.GetActiveDirectoryUserByName(identityName);
				if (loggedInUser != null)
				{
					loggedInUser.IsAdmin = _userRolesDA.IsAdmin(loggedInUser.id);
					loggedInUser.IsPC = _userRolesDA.IsUserPC(loggedInUser.id);
				}
				Session[SK_CURRENT_USER] = loggedInUser;
			}
			else
			{
				loggedInUser = (User)Session[SK_CURRENT_USER];
			}
			ViewBag.LoggedInUser = loggedInUser;
			AppraisalSeason inProgressAppraisalSeason = _appraisalSeasonDa.GetInProgressAppraisalSeason();
			ViewBag.InProgressAppraisalSeason = AutoMapper.Mapper.Map<AppraisalSeasonViewModel>(inProgressAppraisalSeason);
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