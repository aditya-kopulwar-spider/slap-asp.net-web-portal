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
        private NotificationService _notificationService;
        private ActiveDirectoryUserDa _activeDirectoryUserDa=new ActiveDirectoryUserDa();
        public PCController()
        {
            _pcAssociatesDa=new PCAssociatesDA();
            _activeDirectory=new ActiveDirectory();
            _peersDa=new PeersDA();
            _appraisalSeasonDa = new AppraisalSeasonDA();
            _notificationService=new NotificationService();
        }
        // GET: Associates For pc
        public async Task<ActionResult> Index()
        {
            var identityName = User.Identity.Name;
            var users = _activeDirectoryUserDa.GetActiveDirectoryUsers().ToList()
                .Select(p => AutoMapper.Mapper.Map<User>(p)).ToList();
            var adUsersMap = users.ToDictionary(key => key.id, value => value.displayName);
            var userID = users.First(adUser => adUser.userPrincipalName == identityName).id;
            ViewBag.AssociateId = userID;
            var pcAssociateUserViewModels = _pcAssociatesDa.GetAllAssociateForGivenPCForActiveAppraisalSeason(userID).Select(pcAssociate => AutoMapper.Mapper.Map<PCAssociate, PCAssociateViewModel>(pcAssociate)).ToList();
            pcAssociateUserViewModels.ForEach(p => p.AssociateDisplayName = adUsersMap[p.AssociateUserId]);
            pcAssociateUserViewModels.ForEach(p => p.Peers.ForEach(q => q.PeerName = adUsersMap[q.PeerUserId]));
            return View(new PCAssociateViewModels() { PcAssociateViewModels = pcAssociateUserViewModels });
        }

        /*public async Task<ActionResult> Associates(int pcAssociateId)
        {
            var identityName = User.Identity.Name;
            var users = await _activeDirectory.GetAllAdUsers();
            var adUsersMap = users.ToDictionary(key => key.id, value => value.displayName);
            var userID = users.First(adUser => adUser.userPrincipalName == identityName).id;
            ViewBag.AssociateId = userID;
            var pcAssociateUserViewModels =
                AutoMapper.Mapper.Map<PCAssociateViewModel>(_pcAssociatesDa.GetPCAssociate(pcAssociateId));
            pcAssociateUserViewModels.PCDisplayName = adUsersMap[pcAssociateUserViewModels.PCUserId];
            pcAssociateUserViewModels.AssociateDisplayName = adUsersMap[pcAssociateUserViewModels.AssociateUserId];
            pcAssociateUserViewModels.Peers.ForEach(q => q.PeerName = adUsersMap[q.PeerUserId]);
            return View(pcAssociateUserViewModels);
        }*/

        public async Task<ActionResult> AssignPeersToAssociates(PCAssociateViewModel pcAssociateViewModel)
        {
            var peersForGivenAssociateByPcAssociateId = _peersDa.GetPeersForGivenAssociateByPcAssociateId(pcAssociateViewModel.PCAssociatesId);
            var users = _activeDirectoryUserDa.GetActiveDirectoryUsers().ToList()
                .Select(p => AutoMapper.Mapper.Map<User>(p)).ToList();
            var peerViewModels = users.Where(p=>p.id!=pcAssociateViewModel.PCUserId && p.id!=pcAssociateViewModel.AssociateUserId).ToList()
                .Select(p => AutoMapper.Mapper.Map<PeerViewModel>(p)).ToList();
            foreach (var peerViewModel in peerViewModels)
            {
                peerViewModel.AssociateUserId = pcAssociateViewModel.AssociateUserId;
                peerViewModel.IsSelected = false || peersForGivenAssociateByPcAssociateId.Any(p=>p.PeerUserId==peerViewModel.PeerUserId);
                peerViewModel.PCAssociateId = pcAssociateViewModel.PCAssociatesId;
            }
            var associatePeerSelectionModel=new AssociatePeerSelectionModel();
            associatePeerSelectionModel.PeerViewModels.AddRange(peerViewModels);
            associatePeerSelectionModel.PeerListFinalized = pcAssociateViewModel.PeerListFinalized;
            return View(associatePeerSelectionModel);
        }

        public async Task<ActionResult> AddPeers(AssociatePeerSelectionModel peerSelectionModel,string addPeersButton)
        {
           
            var peers = peerSelectionModel.PeerViewModels.Where(p=>p.IsSelected==true)
                .Select(p=>AutoMapper.Mapper.Map<Peer>(p)).ToList();
            if (peers.Any())
            {
                var pcAssociateId = peers.FirstOrDefault().PCAssociateId;
                var peersForGivenAssociateByPcAssociateId = _peersDa.GetPeersForGivenAssociateByPcAssociateId(pcAssociateId);
             _peersDa.RemovePeers(peersForGivenAssociateByPcAssociateId);
                if (addPeersButton == "Finalize Peers")
                {
                    var pcAssociate = _pcAssociatesDa.GetPCAssociate(pcAssociateId);
                    pcAssociate.PeerListFinalized = true;
                    _pcAssociatesDa.EditPCAssociate(pcAssociate);

                    #region mailNotification
                    {
                        var allAdUsers = _activeDirectoryUserDa.GetActiveDirectoryUsers().ToList()
                            .Select(p => AutoMapper.Mapper.Map<User>(p)).ToList();
                        var allAdUsersDictionary = allAdUsers.ToList().ToDictionary(p => p.id);
                        var pc = allAdUsersDictionary[_pcAssociatesDa.GetPCAssociate(pcAssociateId).PCUserId];
                        var associate =
                            allAdUsersDictionary[_pcAssociatesDa.GetPCAssociate(pcAssociateId).AssociateUserId];
                        var peerUsers = new List<User>();
                        peers.ForEach(p=>peerUsers.Add(allAdUsersDictionary[p.PeerUserId]));
                        var activeAppraisalSeason = _appraisalSeasonDa.GetActiveAppraisalSeason();
                        _notificationService.SendMessageToAssociateOnPeerListFinalization(pc,associate,peerUsers);
                       _notificationService.SendMessageToPeersOnPeerListFinalization(pc,associate,peerUsers, activeAppraisalSeason.Name);
                    }
                    #endregion
                }
            }

           
            _peersDa.AddPeers(peers);
            return RedirectToAction("Index","Home");
        }

        public async Task<ActionResult> Associate(Guid associateId)
        {
            ViewBag.AssociateId = associateId;
            var users = _activeDirectoryUserDa.GetActiveDirectoryUsers().ToList()
                .Select(p => AutoMapper.Mapper.Map<User>(p)).ToList();
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
            var _userList = _activeDirectoryUserDa.GetActiveDirectoryUsers().ToList()
                .Select(p => AutoMapper.Mapper.Map<User>(p)).ToList();
            ViewBag.AssociateId = associateId;
            ViewBag.AssociateName = _userList.Find(p => p.id == associateId).displayName;
            //todo Peer table may contain multiple entries for Peers for multiple appraisal seasons
            var allPeers = _peersDa.GetPeersForGivenAssociate(associateId).ToDictionary(k => k.PeerUserId);
            List<PeerAssociateViewModel> pcAssociateUsers = new List<PeerAssociateViewModel>();
            _userList.Where(p=>p.id!=associateId).ForEach(p => pcAssociateUsers.Add(new PeerAssociateViewModel()
            {
//                IsPeer = allPeers.ContainsKey(p.id),
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
            var users = _activeDirectoryUserDa.GetActiveDirectoryUsers().ToList()
                .Select(p => AutoMapper.Mapper.Map<User>(p)).ToList();
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