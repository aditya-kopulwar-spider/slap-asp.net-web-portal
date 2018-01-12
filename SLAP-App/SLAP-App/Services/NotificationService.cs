using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls.WebParts;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using SLAP_App.Models;

namespace SLAP_App.Services
{
    public class NotificationService
    {
        public void SendMessageToAssociateOnPcAssignment(User associate, User pc)
        {
            Email email=new Email();
            email.ToEmail = associate.mail;
            email.CcEmail = pc.mail;
            email.Subject = "PC Assigned";
            email.Message =$"Hi {associate.displayName},<br/> your new PC is {pc.displayName}" ;
            AddMessageToQueue(email);
        }

        private void AddMessageToQueue(Email email)
        {
            email.FromEmail = "donotreply@spiderlogic.com";
            const string connectionString = "Endpoint=sb://slapnotification.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=FewxjX3N2uxck16NOjoa5Xoz/vkNmjWrggUjP9ylfjM=";
            const string queueName = "slapnotificationqueue";
            var client = QueueClient.CreateFromConnectionString(connectionString, queueName);
            string serializeObject = JsonConvert.SerializeObject(email);
            var message = new BrokeredMessage(serializeObject);
            client.Send(message);
//            client.SendBatch(new List<BrokeredMessage>(){message});
        }

        public void SendMessageToAssociateOnPeerListFinalization(User pc, User associate, List<User> peerUsers)
        {
            var email=new Email();
            email.CcEmail = pc.userPrincipalName;
            email.ToEmail = associate.userPrincipalName;
            email.Subject = "PeerList Finalized";
            var peerList = "";
            var index = 0;
            foreach (var peerUser in peerUsers)
            {
                index++;
                peerList = peerList + "\n" + " " + index + ". " + peerUser.displayName;
            }
            email.Message = $"Hi {associate.displayName}, your peer list is finalized. And your peers are: {peerList}";
            AddMessageToQueue(email);
        }

        public void SendMessageToPeersOnPeerListFinalization(User pc, User associate, List<User> peerUsers, string appraisalSeasonName)
        {
             foreach (var peerUser in peerUsers)
            {
                var email = new Email();
                email.CcEmail = pc.userPrincipalName + "; " + associate.userPrincipalName;
                email.Subject = $"Performance Feedback request from {associate.displayName} for {appraisalSeasonName}";

                email.ToEmail = peerUser.userPrincipalName;
                email.Message= $"Hi, {peerUser.displayName} \n \t \t We have started our aanual appraisal process for {appraisalSeasonName}, and {associate.displayName} requests your feedback on his work and contribution this season";
                AddMessageToQueue(email);
            }

           

            



        }
    }
}