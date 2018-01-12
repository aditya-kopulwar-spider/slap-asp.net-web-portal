using SLAP_App.Models;
using SLAP_App.Services;
using SLAP_Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
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
		private PCAssociatesDA _pcAssocaiteDa = new PCAssociatesDA();
		private PeersDA _peersDa = new PeersDA();
		private FileService _fileService = new FileService();

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
			if (inProgressAppraisalSeason != null && inProgressAppraisalSeason.IsActive.GetValueOrDefault())
			{
				var users = await _activeDirectory.GetAllAdUsers();
				var adUsersMap = users.ToDictionary(key => key.id, value => value);
			    var pcAssociateUserViewModels = _pcAssocaiteDa.GetAllAssociateForGivenPCForActiveAppraisalSeason(loggedInUser.id).Select(pcAssociate => AutoMapper.Mapper.Map<PCAssociate, PCAssociateViewModel>(pcAssociate)).ToList();
			    pcAssociateUserViewModels.ForEach(p => p.AssociateDisplayName = adUsersMap[p.AssociateUserId].displayName);
			    pcAssociateUserViewModels.ForEach(p => p.Peers.ForEach(q => q.PeerName = adUsersMap[q.PeerUserId].displayName));
			    ViewBag.PcAssociateUserViewModels = pcAssociateUserViewModels;
                loggedInUser.PCAssociateModel = AutoMapper.Mapper.Map<PCAssociateViewModel>(_pcAssocaiteDa.GetPCAssociateForGivenAssociateId(loggedInUser.id));
				if (loggedInUser.PCAssociateModel != null) loggedInUser.PC = adUsersMap[loggedInUser.PCAssociateModel.PCUserId];
				var seekingFeedbackFrom = _peersDa.GetPeersForGivenAssociate(loggedInUser.id);
				loggedInUser.SeekingFeedbackFrom = seekingFeedbackFrom.Count > 0 ? seekingFeedbackFrom.Select(x => AutoMapper.Mapper.Map<PeerViewModel>(x)).ToList() : null;
				InitUserData(loggedInUser.SeekingFeedbackFrom, adUsersMap);
				var SendingFeedbackTo = _peersDa.GetUsersWhomeGivenAssociateIsPeer(loggedInUser.id);
				loggedInUser.SendingFeedbackTo = SendingFeedbackTo.Count > 0 ? SendingFeedbackTo.Select(x => AutoMapper.Mapper.Map<PeerViewModel>(x)).ToList() : null;
				InitUserData(loggedInUser.SendingFeedbackTo, adUsersMap);
			}
			return View();
        }

		private void InitUserData(IList<PeerViewModel> dataToUpdate, Dictionary<Guid, Models.User> userMap)
		{
			if (dataToUpdate == null) return;

			dataToUpdate.ToList().ForEach(x => {
				x.PeerName = userMap[x.PeerUserId]?.displayName;
				x.AssociateName = userMap[x.AssociateUserId]?.displayName;
			});
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


		[HttpPost]
		public async Task<ActionResult> UpdateFeedback(Guid feedbackFor, string feedbackForName, Guid feedbackFrom, string feedbackFromName, HttpPostedFileBase file, int peerAssociateId, bool shareWithPeer)
		{
			var activeAppraisalProces = _appraisalSeasonDa.GetActiveAppraisalSeason();
			//var name = string.Concat(feedbackForName + "-" + feedbackFromName + "-" + activeAppraisalProces.Name);
			var ext = Path.GetExtension(file.FileName);
			var name = $"[{activeAppraisalProces.Name}]-for-[{feedbackForName}]-from-[{feedbackFromName}]{ext}";

			var path = await _fileService.UploadFile(file, name, activeAppraisalProces.Name);
			var peer = _peersDa.GetByPeerAssociateId(peerAssociateId);
			peer.FeedbackDocumentUrl = path;
			peer.ShareFeedbackWithAssociate = shareWithPeer;
			_peersDa.UpdatePeer(peer);
			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<ActionResult> UpdateSelfAppraisal(Guid feedbackFor, string feedbackForName, HttpPostedFileBase file, int pcAssociateId)
		{
			var pcAssociate = _pcAssocaiteDa.GetPCAssociate(pcAssociateId);
			var appraisalSeason = _appraisalSeasonDa.GetActiveAppraisalSeason();
			var ext = Path.GetExtension(file.FileName);
			var name = $"[{appraisalSeason.Name}]-self-appraisal-for-[{feedbackForName}]{ext}";
			var path = await _fileService.UploadFile(file, name, appraisalSeason.Name);
			pcAssociate.SelfAppraisalDocumentUrl = path;
			_pcAssocaiteDa.EditPCAssociate(pcAssociate);
			return RedirectToAction("Index");
		}
	}
}