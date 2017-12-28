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
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SLAP_App.Services;

namespace SLAP_App.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private UserRolesDA _userRolesDa=new UserRolesDA();
       private PCAssociatesDA _pcAssociatesDa=new PCAssociatesDA();
        ActiveDirectory _activeDirectory=new ActiveDirectory();

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

        public async Task<ActionResult> AssignAssociates(System.Guid pcID)
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
            var pcAssociateUserViewModels = pcAssociateUsers.Where(p => (p.PCUserId == pcID || p.PCUserId == Guid.Empty) &&(p.AssociateUserId!=pcID)).ToList();
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
            return RedirectToAction("AssignAssociates", new{pcID=pcId});
        }
        public ActionResult RemoveAssociate(Guid associateId, Guid pcId)
        {
            _pcAssociatesDa.RemoveAssociate(associateId, pcId);
            return RedirectToAction("AssignAssociates", new { pcID = pcId });
        }
        
        // Retrive AD Users

       
       


    }
}