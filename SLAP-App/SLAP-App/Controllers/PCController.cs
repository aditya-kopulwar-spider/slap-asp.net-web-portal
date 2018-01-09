using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SLAP_App.Models;
using SLAP_App.Services;
using SLAP_Data;
using WebGrease.Css.Extensions;

namespace SLAP_App.Controllers
{
    [Authorize]
    public class PCController : Controller
    {
        private PCAssociatesDA _pcAssociatesDa;
        private PeersDA _peersDa;
        private ActiveDirectory _activeDirectory;
        private AppraisalSeasonDA _appraisalSeasonDa;
        public PCController()
        {
            _pcAssociatesDa=new PCAssociatesDA();
            _activeDirectory=new ActiveDirectory();
            _peersDa=new PeersDA();
            _appraisalSeasonDa = new AppraisalSeasonDA();
        }
        // GET: Associates For pc
        public async Task<ActionResult> Index()
        {
            var activeAppraisalSeason = _appraisalSeasonDa.GetActiveAppraisalSeason();
            if (activeAppraisalSeason==null)
            {
                await AssignPeer();
            }
            var identityName = User.Identity.Name;
            var users = await _activeDirectory.GetAllAdUsers();
            var adUsersMap = users.ToDictionary(key => key.id, value => value.displayName);
            var userID = users.First(adUser => adUser.userPrincipalName == identityName).id;
            ViewBag.AssociateId = userID;
            var pcAssociateUserViewModels = _pcAssociatesDa.GetAllAssociateForGivenPC(userID).Select(pcAssociate => AutoMapper.Mapper.Map<PCAssociate, PCAssociateViewModel>(pcAssociate)).ToList();
            pcAssociateUserViewModels.ForEach(p => p.AssociateDisplayName = adUsersMap[p.AssociateUserId]);
            pcAssociateUserViewModels.ForEach(p => p.Peers.ForEach(q => q.PeerName = adUsersMap[q.PeerUserId]));

            return View(new PCAssociateViewModels() { PcAssociateViewModels = pcAssociateUserViewModels });
        }

        public async Task<ActionResult> AssignPeersToAssociates()
        {

        }

        public async Task<ActionResult> Associate(Guid associateId)
        {
            ViewBag.AssociateId = associateId;
            var users = await _activeDirectory.GetAllAdUsers();
            var userDictionary = users.ToDictionary(p=>p.id);
            ViewBag.AssociateName = userDictionary[associateId].displayName;
            var peerViewModels = _peersDa.GetPeersForGivenAssociate(associateId).Select(p=>AutoMapper.Mapper.Map<Peer,PeerViewModel>(p)).ToList();
            peerViewModels.ForEach(p => p.PeerName = userDictionary[p.PeerUserId].displayName);
            var userListwhomeGivenAssociateIsPeer = _peersDa.GetUsersWhomeGivenAssociateIsPeer(associateId).Select(p => AutoMapper.Mapper.Map<Peer, PeerViewModel>(p)).ToList();
            userListwhomeGivenAssociateIsPeer.ForEach(p => p.PeerName = userDictionary[p.AssociateUserId].displayName);
            return View(peerViewModels.Concat(userListwhomeGivenAssociateIsPeer));
        }
        
        public async Task<ActionResult> AddNewPeer(Guid associateId)
        {
            var _userList = await _activeDirectory.GetAllAdUsers();
            ViewBag.AssociateId = associateId;
            ViewBag.AssociateName = _userList.Find(p => p.id == associateId).displayName;
            //todo Peer table may contain multiple entries for Peers for multiple appraisal seasons
            var allPeers = _peersDa.GetPeersForGivenAssociate(associateId).ToDictionary(k => k.PeerUserId);
            List<PeerAssociateViewModel> pcAssociateUsers = new List<PeerAssociateViewModel>();
            _userList.Where(p=>p.id!=associateId).ForEach(p => pcAssociateUsers.Add(new PeerAssociateViewModel()
            {
                IsPeer = allPeers.ContainsKey(p.id),
                PeerId = p.id,
                AssociateUserId = associateId,
                PeerDisplayName = p.displayName
            }));
//            var pcAssociateUserViewModels = pcAssociateUsers.Where(p => (p.PCUserId == pcID || p.PCUserId == Guid.Empty) && (p.AssociateUserId != pcID)).ToList();
            return View(pcAssociateUsers);
        }

        public ActionResult MakePeer(Guid associateId, Guid peerId)
        {
           Peer peer =new Peer()
           {
               PeerUserId = peerId,
               AssociateUserId = associateId
           };
            _peersDa.AddPeer(peer);
            return RedirectToAction("AddNewPeer", new { associateId = associateId });
        }
        public  ActionResult RemovePeer(Guid associateId, Guid peerId)
        {
           
            _peersDa.RemovePeer(associateId,peerId);
            return RedirectToAction("AddNewPeer", new { associateId = associateId });
        }
        
        public async Task<ActionResult> EditAssociate(int? pcAssociateId)
        {
            if (pcAssociateId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PCAssociate pcAssociate = _pcAssociatesDa.GetPCAssociate(pcAssociateId);
            if (pcAssociate == null)
            {
                return HttpNotFound();
            }
            var pcAssociateViewModel = AutoMapper.Mapper.Map<PCAssociate, PCAssociateViewModel>(pcAssociate);
            var users = await _activeDirectory.GetAllAdUsers();
            pcAssociateViewModel.AssociateDisplayName =
                users.First(p => p.id == pcAssociateViewModel.AssociateUserId).displayName;
            return View(pcAssociateViewModel);
        }
        [HttpPost]
        public ActionResult EditAssociate(PCAssociateViewModel pcAssociateViewModel)
        {
            if (ModelState.IsValid)
            {
                _pcAssociatesDa.EditPCAssociate(
                    AutoMapper.Mapper.Map<PCAssociateViewModel, PCAssociate>(pcAssociateViewModel));
                return RedirectToAction("Index");
            }
            return View(pcAssociateViewModel);
        }
    }
}