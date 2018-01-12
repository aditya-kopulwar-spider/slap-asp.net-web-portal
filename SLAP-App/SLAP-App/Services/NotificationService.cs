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
            var email = new Email
            {
                ToEmail = associate.mail,
                CcEmail = pc.mail,
                Subject = "PC Assigned",
                Message = $"Hi {associate.displayName},<br/> your new PC is {pc.displayName}"
            };
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
            var email = new Email
            {
                CcEmail = pc.userPrincipalName,
                ToEmail = associate.userPrincipalName,
                Subject = "PeerList Finalized"
            };
            var peerList = "";
            var index = 0;
            foreach (var peerUser in peerUsers)
            {
                index++;
                peerList = peerList + "\n" + " " + index + ". " + peerUser.displayName;
            }
            email.Message = $"Hi {associate.displayName}, your peer list is finalized. And your peers are:" +
                            $" {peerList}";
            AddMessageToQueue(email);
        }

        public void SendMessageToPeersOnPeerListFinalization(User pc, User associate, List<User> peerUsers, 
            string appraisalSeasonName)
        {
             foreach (var peerUser in peerUsers)
            {
                var email = new Email
                {
                    CcEmail = pc.userPrincipalName + "; " + associate.userPrincipalName,
                    Subject = $"Performance Feedback request from {associate.displayName} " +
                              $"for {appraisalSeasonName}",
                    ToEmail = peerUser.userPrincipalName,
                    Message = $"Hi, {peerUser.displayName} \n \t \t We have started our aanual appraisal process for" +
                              $" {appraisalSeasonName}, and {associate.displayName} " +
                              $"requests your feedback on his work and contribution this season"
                };

                AddMessageToQueue(email);
            }
        }

        public void SendMesageToAssociateOnPcAssignment(User pc, User associate, string appraisalSeasonName)
        {
            var email = new Email
            {
                ToEmail = associate.userPrincipalName,
                CcEmail = pc.userPrincipalName,
                Subject = $"PC Assignemnt for Appraisal Season {appraisalSeasonName}",
                Message =
                    $"Hi {associate.displayName}, \n\t Your PC for appraisal season {appraisalSeasonName} is {pc.displayName}. " +
                    $"Please contact your PC({pc.displayName}) to finalize peer list."
            };
            AddMessageToQueue(email);
        }

        public void SendMesageToPcOnAssociateAssignment(User pcUser, List<User> associates, string appraisalSeasonName)
        {
            var email=new Email();
            email.ToEmail = pcUser.userPrincipalName;
            email.Subject = $"Associate Assignment for Appraisal Season {appraisalSeasonName}";
            var associateNames = "";
            var index = 0;
            foreach (var associate in associates)
            {
                index++;
                if (associates.Count==1)
                {
                    associateNames ="\n" + associate.displayName;
                }
                associateNames = associateNames + "\n" + index + ". " + associate.displayName;
            }
            email.Message = $"Hi {pcUser.displayName}, \n\t\t" +
                            $"You have been assigned as PC for appraisal season {appraisalSeasonName}" +
                            $" for following associates: \n\t\t{associateNames}";
            AddMessageToQueue(email);
        }
    }
}