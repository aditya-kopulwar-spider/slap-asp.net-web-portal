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

        // GET: Admin
        public async Task<ActionResult> Index()
        {
            List<User> userList = await _activeDirectory.GetAllAdUsers();
            userList.ForEach(p => p.IsPC = _userRolesDa.IsUserPC(p.id));
            return View(userList);
        }



        public ActionResult MakePC(System.Guid Id)
        {
            _userRolesDa.MakeUserPC(Id);
            return RedirectToAction("Index");
        }

        public ActionResult RemovePC(System.Guid Id)
        {
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

        public async Task<ActionResult> AssignAssociates(Guid? pcId)
        {
            var _userList = await _activeDirectory.GetAllAdUsers();
            ViewBag.PCId = pcId;
            ViewBag.pcName = _userList.First(p => p.id == pcId).displayName;
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
                .Where(p => (p.PCUserId == pcId || p.PCUserId == Guid.Empty) && (p.AssociateUserId != pcId)).ToList();
            pcAssociateUserViewModels.ForEach(p=>p.PCUserId=(Guid) pcId);
            AssociateSelectionViewModel associateSelectionViewModel=new AssociateSelectionViewModel();
            associateSelectionViewModel.PcAssociateUserViewModels = pcAssociateUserViewModels;
            return View(associateSelectionViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> AssignAssociates(AssociateSelectionViewModel associateSelectionViewModel)
        {
            //            var selectedAssociates = id.getSelectedAssociates();
            //todo approach to assign associates only one time activity or ---
            var pcUserId = associateSelectionViewModel.PcAssociateUserViewModels.FirstOrDefault(p=>p.Selected).PCUserId;
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




    }
}