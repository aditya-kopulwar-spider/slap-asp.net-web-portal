using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using SLAP_App.Models;

namespace SLAP_App.Services
{
    public class ActiveDirectory
    {
        public  async Task<List<User>> GetAllAdUsers()
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
        private static async Task<string> AppAuthenticationAsync()
        {

            string clientID = ConfigurationManager.AppSettings["ida:ClientId"];
            string tenant = ConfigurationManager.AppSettings["ida:TenantId"];
            string secret = ConfigurationManager.AppSettings["ida:ClientSecret"];
            var resource = "https://graph.microsoft.com/";

            //  Ceremony
            var authority = $"https://login.microsoftonline.com/{tenant}";
            var authContext = new AuthenticationContext(authority);
            var credentials = new ClientCredential(clientID, secret);
            var authResult = await authContext.AcquireTokenAsync(resource, credentials);

            return authResult.AccessToken;
        }


    }
}