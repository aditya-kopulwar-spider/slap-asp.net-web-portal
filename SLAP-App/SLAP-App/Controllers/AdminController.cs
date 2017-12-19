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

namespace SLAP_App.Controllers
{
    public class AdminController : Controller
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();

        // GET: Admin
        public ActionResult Index()
        {
            List<ADUser> userList = new List<ADUser>
            {
                new ADUser { Id = System.Guid.NewGuid(), Name = "Tina" },
                new ADUser { Id = System.Guid.NewGuid(), Name = "Ameya" },
                new ADUser { Id = System.Guid.NewGuid(), Name = "Himanshu" },
                new ADUser { Id = System.Guid.NewGuid(), Name = "Rasika", IsPC = true },
            };

            return View(userList);
        }

        public string GetUsers()
        {
            return GetAllAdUsers().Result;
        }
        
        public ActionResult MakePC(System.Guid Id)
        {
            dbHelper.MakeUserPC(Id);
            return RedirectToAction("Index");
        }

        public ActionResult RemovePC(System.Guid Id)
        {
            dbHelper.RemoveUserFromPCRole(new System.Guid("1B33D032-C208-43E7-A825-3AC58E9799D1"));
            return RedirectToAction("Index");
        }

        // Retrive AD Users

        private static async Task<string> AppAuthenticationAsync()
        {

            //string clientID = ConfigurationManager.AppSettings["ida:ClientId"];
            //string tenant = ConfigurationManager.AppSettings["ida:TenantId"];
            //string secret = ConfigurationManager.AppSettings["ida:ClientSecret"];
            //var resource = "https://graph.microsoft.com/";

            var tenant = "ajaychavan1312outlook.onmicrosoft.com";
            var clientID = "03d41270-0bc5-4806-af8f-8a89c54206dc";
            var resource = "https://graph.microsoft.com/";
            var secret = "dm7sfMC3cIRkax0y9xUNE6F9M7SRe8At5x/iQIsZiPE=";

            //  Ceremony
            var authority = $"https://login.microsoftonline.com/{tenant}";
            var authContext = new AuthenticationContext(authority);
            var credentials = new ClientCredential(clientID, secret);
            var authResult = await authContext.AcquireTokenAsync(resource, credentials);

            return authResult.AccessToken;
        }

        private static async Task<string> GetAllAdUsers()
        {
            var token = await AppAuthenticationAsync();
            //var token = await HttpAppAuthenticationAsync();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var allUsers = await client.GetStringAsync("https://graph.microsoft.com/v1.0/users/");

                return allUsers;
            }
        }

    }
}