using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using SLAP_App.Models;

namespace SLAP_App.Services
{
    public class NotificationService
    {
        public void SendMessageToAssociateOnPcAssignment(User associate,User pc)
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
            const string connectionString = "Endpoint=sb://slap-app.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=/eF64fvC1DN6eZ+z+H9yOXQMNhqW5xB3MhPbAxPl5PQ=";
            const string queueName = "slapmailnotificationqueue";
            var client = QueueClient.CreateFromConnectionString(connectionString, queueName);
            string serializeObject = JsonConvert.SerializeObject(email);
            var message = new BrokeredMessage(serializeObject);
            client.Send(message);
        }
    }
}