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

namespace SLAP_App.Controllers
{
    public class AdminController : Controller
    {
        private UserRolesDA _userRolesDa=new UserRolesDA();
       private PCAssociatesDA _pcAssociatesDa=new PCAssociatesDA();

        // GET: Admin
        public async Task<ActionResult> Index()
        {
            List<User> userList = await GetUsersAsync();
            userList.ForEach(p => p.IsPC = _userRolesDa.IsUserPC(p.id));
            return View(userList);
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return  await GetAllAdUsers();
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
            var _userList = await GetAllAdUsers();
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

        private static async Task<string> AppAuthenticationAsync()
        {

            string clientID = ConfigurationManager.AppSettings["ida:ClientId"];
            string tenant = ConfigurationManager.AppSettings["ida:TenantId"];
            string secret = ConfigurationManager.AppSettings["ida:ClientSecret"];
            var resource = "https://graph.microsoft.com/";

//            var tenant = "ajaychavan1312outlook.onmicrosoft.com";
//            var clientID = "03d41270-0bc5-4806-af8f-8a89c54206dc";
//            var resource = "https://graph.microsoft.com/";
//            var secret = "dm7sfMC3cIRkax0y9xUNE6F9M7SRe8At5x/iQIsZiPE=";

            //  Ceremony
            var authority = $"https://login.microsoftonline.com/{tenant}";
            var authContext = new AuthenticationContext(authority);
            var credentials = new ClientCredential(clientID, secret);
            var authResult = await authContext.AcquireTokenAsync(resource, credentials);

            return authResult.AccessToken;
        }

        private static async Task<List<User>> GetAllAdUsers()
        {
            var token = await AppAuthenticationAsync();
            //var token = await HttpAppAuthenticationAsync();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var allUsers = await client.GetStringAsync("https://graph.microsoft.com/v1.0/users/");
                var result = JsonConvert.DeserializeObject<RootObject>(allUsers);
                return result.Users;
            }
        }


    }
}