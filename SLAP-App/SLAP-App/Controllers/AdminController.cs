using System;
using SLAP_App.Models;
using System.Web.Mvc;
using System.Collections.Generic;
using SLAP_Data;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;
using System.Drawing;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SLAP_App.Services;

namespace SLAP_App.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private UserRolesDA _userRolesDa = new UserRolesDA();
        private PCAssociatesDA _pcAssociatesDa = new PCAssociatesDA();
        private NotificationService _notificationService = new NotificationService();
        ActiveDirectory _activeDirectory = new ActiveDirectory();
		private AppraisalSeasonDA _appraisalSeasonDa = new AppraisalSeasonDA();
        private ActiveDirectoryUserDa _activeDirectoryUserDa=new ActiveDirectoryUserDa();
		// GET: Admin
		public  ActionResult Index()
        {
			if (_appraisalSeasonDa.GetActiveAppraisalSeason() != null) return RedirectToAction("Index", "Home");
            List<User> userList = _activeDirectoryUserDa.GetActiveDirectoryUsers().ToList()
                .Select(p => AutoMapper.Mapper.Map<User>(p)).ToList();
            userList.ForEach(p => p.IsPC = _userRolesDa.IsUserPC(p.id));
            return View(userList);
        }
		
        public ActionResult MakePC(System.Guid Id)
        {
			if (_appraisalSeasonDa.GetActiveAppraisalSeason() != null) return RedirectToAction("Index", "Home");
            _userRolesDa.MakeUserPC(Id);
			return RedirectToAction("Index");
        }

        public ActionResult RemovePC(System.Guid Id)
        {
			if (_appraisalSeasonDa.GetActiveAppraisalSeason() != null) return RedirectToAction("Index", "Home");
            _userRolesDa.RemoveUserFromPCRole(Id);
            return RedirectToAction("Index");
        }

       /* public async Task<ActionResult> AssignAssociates(System.Guid pcID)
        {
            var _userList = await _activeDirectory.GetAllAdUsers();
            ViewBag.PCId = pcID;
            //todo PCAssociate table may contain multiple entries for associate for multiple appraisal seasons
            var allPcAssociates = _pcAssociatesDa.GetAllPcAssociates().ToDictionary(k => k.AssociateUserId);
            List<PCAssociateUserViewModel> pcAssociateUsers = new List<PCAssociateUserViewModel>();
            _userList.ForEach(p => pcAssociateUsers.Add(new PCAssociateUserViewModel()
            {
                PCUserId = allPcAssociates.ContainsKey(p.id) ? allPcAssociates[p.id].PCUserId : Guid.Empty,
                AssociateUserId = p.id,
                AssociateDisplayName = p.displayName
            }));
            var pcAssociateUserViewModels = pcAssociateUsers
                .Where(p => (p.PCUserId == pcID || p.PCUserId == Guid.Empty) && (p.AssociateUserId != pcID)).ToList();
            return View(pcAssociateUserViewModels);
        }

        public ActionResult MakeAssociate(Guid associateId, Guid pcId)
        {
            SLAP_Data.PCAssociate pcAssociate = new SLAP_Data.PCAssociate()
            {
                PCUserId = pcId,
                AssociateUserId = associateId
            };

            _pcAssociatesDa.AddAsociate(pcAssociate);

            _notificationService.SendMessageToAssociateOnPcAssignment(
                new User {displayName = "Associate Name", mail = "kshah@spiderlogic.com"},
                new User {displayName = "PC Name", mail = "kshah@spiderlogic.com"});

            return RedirectToAction("AssignAssociates", new {pcID = pcId});
        }

        public ActionResult RemoveAssociate(Guid associateId, Guid pcId)
        {
            _pcAssociatesDa.RemoveAssociate(associateId, pcId);
            return RedirectToAction("AssignAssociates", new {pcID = pcId});
        }*/

        #region newAssociatesScreen

        public ActionResult AssignAssociates(Guid? pcId)
        {
			if (_appraisalSeasonDa.GetActiveAppraisalSeason() != null) return RedirectToAction("Index", "Home");
            var _userList = _activeDirectoryUserDa.GetActiveDirectoryUsers().ToList()
                .Select(p => AutoMapper.Mapper.Map<User>(p)).ToList();
            ViewBag.PCId = pcId;
            ViewBag.pcName = _userList.First(p => p.id == pcId).displayName;
            var pcAssociate = _pcAssociatesDa.GetPCAssociateForGivenAssociateId((Guid) pcId);
            var pcAsoociatePcUserId=Guid.Empty;
            if (pcAssociate!=null)
            {
                 pcAsoociatePcUserId = pcAssociate.PCUserId;
            }
            var allPcAssociates = _pcAssociatesDa.GetAllPcAssociatesForInProgressAppraisalSeason().ToDictionary(k => k.AssociateUserId);
            List<PCAssociateUserViewModel> pcAssociateUsers = new List<PCAssociateUserViewModel>();
            _userList.ForEach(p => pcAssociateUsers.Add(new PCAssociateUserViewModel()
            {
                PCUserId = allPcAssociates.ContainsKey(p.id) ? allPcAssociates[p.id].PCUserId : Guid.Empty,
                Selected = allPcAssociates.ContainsKey(p.id),
                AssociateUserId = p.id,
                AssociateDisplayName = p.displayName
            }));

            var pcAssociateUserViewModels = pcAssociateUsers
                .Where(p => (p.PCUserId == pcId || p.PCUserId == Guid.Empty) && (p.AssociateUserId != pcId) &&(pcAsoociatePcUserId!=p.AssociateUserId)).ToList();
            pcAssociateUserViewModels.ForEach(p=>p.PCUserId=(Guid) pcId);
            AssociateSelectionViewModel associateSelectionViewModel=new AssociateSelectionViewModel();
            associateSelectionViewModel.PcAssociateUserViewModels = pcAssociateUserViewModels;
            return View(associateSelectionViewModel);
        }

        [HttpPost]
        public ActionResult AssignAssociates(AssociateSelectionViewModel associateSelectionViewModel, Guid pcId)
        {
			if (_appraisalSeasonDa.GetActiveAppraisalSeason() != null) return RedirectToAction("Index", "Home");
			//            var selectedAssociates = id.getSelectedAssociates();
			//todo approach to assign associates only one time activity or ---
			//var pcUserId = associateSelectionViewModel.PcAssociateUserViewModels.FirstOrDefault(p=>p.Selected).PCUserId;
			var pcUserId = pcId;
            var allCurrentYearPcAssociatesForGivenPcId = _pcAssociatesDa.GetAllPcAssociatesForPcIdForInProgressAppraisalSeason(pcUserId);
            _pcAssociatesDa.RemoveAssociates(allCurrentYearPcAssociatesForGivenPcId);
            var pcAssociates = associateSelectionViewModel.PcAssociateUserViewModels.Where(p => p.Selected).ToList()
                .Select(p => AutoMapper.Mapper.Map<PCAssociate>(p))
                .ToList();
            var addAsociates = _pcAssociatesDa.AddAssociates(pcAssociates);

            //if (addAsociates)
            //{
            //    var allAdUsers =await _activeDirectory.GetAllAdUsers();
            //    var allAdUsersDictionary  = allAdUsers.ToList().ToDictionary(p => p.id);
            //    foreach (var pcAssociate in pcAssociates)
            //    {
            //        var associate = allAdUsersDictionary[pcAssociate.AssociateUserId];
            //        var pc = allAdUsersDictionary[pcAssociate.PCUserId];
            //        _notificationService.SendMessageToAssociateOnPcAssignment(associate, pc);
            //    }
               
            //}
            

            return RedirectToAction("Index");
        }

        #endregion

        #region add initial pc associate relation

        public void CreateInitialPCAssociateRelation()
        {
            var activeDirectoryUserViewModels = _activeDirectoryUserDa.GetActiveDirectoryUsers()
                .Select(p => AutoMapper.Mapper.Map<ActiveDirectoryUserViewModel>(p));
            var pcAssociates = new List<PCAssociate>();
            var _userRoles=new List<UserRole>();
            foreach (var activeDirectoryUserViewModel in activeDirectoryUserViewModels
                .Where(p=>p.Associates.Any()).ToList())
            {
                _userRoles.Add(new UserRole()
                {
                    UserId = activeDirectoryUserViewModel.Id
                });
                foreach (var associate  in activeDirectoryUserViewModel.Associates)
                {
                pcAssociates.Add(new PCAssociate()
                {
                    PCUserId = activeDirectoryUserViewModel.Id,
                    AssociateUserId = associate.Id,
                    PeerListFinalized = false
                });    
                }
            }
            _pcAssociatesDa.AddAssociates(pcAssociates);
            _userRolesDa.MakeUsersPc(_userRoles);
        }

        #endregion



    }
}