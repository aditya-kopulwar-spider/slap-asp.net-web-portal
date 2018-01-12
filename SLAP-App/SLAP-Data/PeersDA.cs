using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLAP_Data
{
   public class PeersDA
    {
        private slap_dbEntities _dbEntities;
        private AppraisalSeasonDA _appraisalSeasonDa;
        private PCAssociatesDA _pcAssociatesDa;
        public PeersDA()
        {
            _dbEntities=new slap_dbEntities();
            _appraisalSeasonDa=new AppraisalSeasonDA();
            _pcAssociatesDa=new PCAssociatesDA();
        }

        //todo method name change it returns  1) peers for given associateId 2)associates to whome given id is a peer
        public List<Peer> GetAllPeersForGivenAssociate(Guid associateId)
        {
            //todo take active appraisal yaear instead of inprogess 
           var activeAppraisalSeason = _appraisalSeasonDa.GetInProgressAppraisalSeason();
           var pcAssociates=    _pcAssociatesDa.GetAllPcAssociatesForGivenAppraisalSeason(activeAppraisalSeason.AppraisalSeasonId);
           var peers=new List<Peer>();
            pcAssociates.ForEach(p=>peers.AddRange(p.Peers));
           return peers.Where(p => p.PeerUserId == associateId || p.AssociateUserId == associateId).ToList();
//            return _dbEntities.Peers.Where(p => p.AssociateUserId == associateId || p.PeerUserId == associateId).ToList();
        }
        public List<Peer> GetPeersForGivenAssociate(Guid associateId)
        {
            return _dbEntities.Peers.Where(p => p.AssociateUserId == associateId).ToList();
        }
        public List<Peer> GetUsersWhomeGivenAssociateIsPeer(Guid associateId)
        {
            return _dbEntities.Peers.Where(p => p.PeerUserId == associateId && p.PCAssociate.PeerListFinalized).ToList();
        }
        public bool AddPeer(Peer peer)
        {
            var appraisalProcesses = _dbEntities.AppraisalSeasons.First(p => p.IsActive == true);
//            peer.AppraisalSeasonId = appraisalProcesses.AppraisalSeasonId;
//            peer.FeedbackStatus = false;
            _dbEntities.Peers.Add(peer);
            _dbEntities.SaveChanges();
            return true;
        }

        public bool RemovePeer(Guid associateId,Guid peerId)
        {
            var peer = _dbEntities.Peers.FirstOrDefault(p => p.AssociateUserId == associateId && p.PeerUserId == peerId);
            if (peer != null) _dbEntities.Peers.Remove(peer);
            _dbEntities.SaveChanges();
            return true;
        }

        public bool UpdatePeer(Peer peer)
        {
            _dbEntities.Entry(peer).State = EntityState.Modified;
            _dbEntities.SaveChanges();
            return true;
        }
        public List<Peer> GetPeersForGivenAssociateByPcAssociateId(int pcAssociateId)
        {
            return _dbEntities.Peers.Where(p => p.PCAssociateId==pcAssociateId).ToList();
        }
       
        public bool AddPeers(IEnumerable<Peer> peers)
        {
            _dbEntities.Peers.AddRange(peers);
            _dbEntities.SaveChanges();
            return true;
        }

        public bool RemovePeers(List<Peer> peersForGivenAssociateByPcAssociateId)
        {
            _dbEntities.Peers.RemoveRange(peersForGivenAssociateByPcAssociateId);
            _dbEntities.SaveChanges();
            return true;    
        }

		public Peer GetByPeerAssociateId(int peerAssociateId)
		{
			return _dbEntities.Peers.Where(p => p.PeerAssociateId == peerAssociateId).FirstOrDefault();
		}
	}
}
