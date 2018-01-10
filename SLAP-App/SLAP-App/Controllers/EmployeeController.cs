using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SLAP_App.Models;
using SLAP_App.Services;
using SLAP_Data;

namespace SLAP_App.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ActiveDirectory _activeDirectory=new ActiveDirectory();
        private AppraisalSeasonDA _appraisalProcessDa=new AppraisalSeasonDA();
        private PCAssociatesDA _pcAssocaiteDa = new PCAssociatesDA();
        private FileService _fileService = new FileService();
        private PeersDA _peersDa=new PeersDA();
        // GET: Employee
        public async Task<ActionResult> Index()
        {
            var identityName = User.Identity.Name;
            var users = await _activeDirectory.GetAllAdUsers();
            var adUsersMap = users.ToDictionary(key => key.id, value => value.displayName);
            var userID = users.First(adUser => adUser.userPrincipalName == identityName).id;
            ViewBag.UserID = userID;
            ViewBag.AssociateId = userID;
            var peersForGivenAssociate = _peersDa.GetAllPeersForGivenAssociate(userID);
            var employeeViewModels = peersForGivenAssociate
                .Select(AutoMapper.Mapper.Map<EmployeeViewModel>)
                .ToList();
            var pcAssociateForGivenAssociateId =
                AutoMapper.Mapper.Map<PCAssociateViewModel>(_pcAssocaiteDa.GetPCAssociateForGivenAssociateId(userID));
            employeeViewModels.ForEach(p=>p.PeerName=adUsersMap[p.PeerUserId]);
            employeeViewModels.ForEach(p=>p.AssociateName=adUsersMap[p.AssociateUserId]);
            var viewModels = new EmployeeViewModels()
            {
                EmployeeModels = employeeViewModels,
                PcAssociateViewModel = pcAssociateForGivenAssociateId
            };
            return View(viewModels);
        }
        [HttpPost]
        public async Task<ActionResult> UpdateFeedback(EmployeeViewModel employeeViewModel)
        {
            var activeAppraisalProces = _appraisalProcessDa.GetActiveAppraisalSeason();
            var name = string.Concat(employeeViewModel.AssociateName + "-" + employeeViewModel.PeerName + "-" + activeAppraisalProces.Name);
            
            var path=  await _fileService.UploadFile(employeeViewModel.FeedbackDocument, name,activeAppraisalProces.Name);
            employeeViewModel.FeedbackDocumentUrl = path;
            var peer = AutoMapper.Mapper.Map<Peer>(employeeViewModel);
            _peersDa.UpdatePeer(peer);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public  ActionResult UpdateSelfAppraisal(PCAssociateViewModel pcAssociateViewModel)
        {
            return RedirectToAction("Index");
        }
    }
}