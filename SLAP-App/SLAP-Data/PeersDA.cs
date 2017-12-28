using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLAP_Data
{
   public class PeersDA
    {
        private slap_dbEntities _dbEntities;

        public PeersDA()
        {
            _dbEntities=new slap_dbEntities();
        }

        //todo method name change it returns  1) peers for given associateId 2)associates to whome given id is a peer
        public List<Peer> GetAllPeersForGivenAssociate(Guid associateId)
        {
           return _dbEntities.Peers.Where(p => p.AssociateUserId == associateId || p.PeerUserId == associateId).ToList();
        }
        public List<Peer> GetPeersForGivenAssociate(Guid associateId)
        {
            return _dbEntities.Peers.Where(p => p.AssociateUserId == associateId).ToList();
        }
        public List<Peer> GetUsersWhomeGivenAssociateIsPeer(Guid associateId)
        {
            return _dbEntities.Peers.Where(p => p.PeerUserId == associateId).ToList();
        }
        public bool AddPeer(Peer peer)
        {
            var appraisalProcesses = _dbEntities.AppraisalProcesses.First(p => p.IsActive == true);
            peer.AppraisalProcessId = appraisalProcesses.AppraisalProcessId;
            peer.FeedbackStatus = false;
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
    }
}
