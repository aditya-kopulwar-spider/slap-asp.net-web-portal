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
using SLAP_Data;

namespace SLAP_App.Services
{
    public class ActiveDirectory
    {
        //todo change method name and return type
       /* public async Task<List<User>> GetAllAdUsers()
        {
            var token = await AppAuthenticationAsync();
            //var token = await HttpAppAuthenticationAsync();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var allUsers = await client.GetStringAsync("https://graph.microsoft.com/v1.0/users?&$filter=accountEnabled eq true");
                var result = JsonConvert.DeserializeObject<RootObject>(allUsers);
                return result.Users;
            }
        }*/

        public async Task StoreAdUSersInTable()
        {
            var token = await AppAuthenticationAsync();
            //var token = await HttpAppAuthenticationAsync();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var allUsers =
                    await client.GetStringAsync(
                        "https://graph.microsoft.com/v1.0/users?&$filter=accountEnabled eq true");
                var result = JsonConvert.DeserializeObject<RootObject>(allUsers);
                foreach (var user in result.Users)
                {
                    var activeDirectoryUserManager = await GetActiveDirectoryUserManager(user.userPrincipalName);
                    if (result.Users.Any(p => p.id == activeDirectoryUserManager.id))
                    {
                        user.Manager = activeDirectoryUserManager;
                    }
                }
                var activeDirectoryUserViewModels = new List<ActiveDirectoryUser>();
                activeDirectoryUserViewModels.AddRange(result.Users.Select(p =>
                    AutoMapper.Mapper.Map<ActiveDirectoryUser>(p)));
                var activeDirectoryUserDa = new ActiveDirectoryUserDa();
                activeDirectoryUserViewModels.ForEach(p => p.ManagerId = null);
                //remove previous all ad users from table
                activeDirectoryUserDa.RemoveAllActiveDirectoryUsers();
                activeDirectoryUserDa.AddActiveDirectoryUsers(activeDirectoryUserViewModels);
            }
        }

       /* public async Task<User> GetActiveDirectoryUserByName(string name)
		{
			var token = await AppAuthenticationAsync();
			//var token = await HttpAppAuthenticationAsync();

			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var user = await client.GetStringAsync($"https://graph.microsoft.com/v1.0/users/{name}");
				var result = JsonConvert.DeserializeObject<User>(user);
				return result;
			}
		}*/
        public async Task<User> GetActiveDirectoryUserManager(string userId)
        {
            var token = await AppAuthenticationAsync();
            //var token = await HttpAppAuthenticationAsync();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                try
                {
                    var user = await client.GetStringAsync($"https://graph.microsoft.com/beta/users/{userId}/manager");
                    return JsonConvert.DeserializeObject<User>(user);
                }
                catch (Exception e)
                {
                    return new User();
                }
                
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