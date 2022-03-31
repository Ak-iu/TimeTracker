using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;

namespace TimeTracker.Apps.Api
{
    public class Authentication
    {
        public async Task<HttpResponseMessage> Refresh(string refreshToken)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://timetracker.julienmialon.ovh");

            JObject jsonData = new JObject(
                new JProperty("refresh_token", refreshToken),
                new JProperty("client_id", "MOBILE"),
                new JProperty("client_secret", "COURS"));
            var content = new StringContent(jsonData.ToString(), Encoding.UTF8, "application/json");

            
            return await client.PostAsync("api/v1/refresh", content);
        }
        public async Task<HttpResponseMessage> Login(string email, string password)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://timetracker.julienmialon.ovh");

            JObject jsonData = new JObject(
                new JProperty("login", email),
                new JProperty("password", password),
                new JProperty("client_id", "MOBILE"),
                new JProperty("client_secret", "COURS"));
            var content = new StringContent(jsonData.ToString(), Encoding.UTF8, "application/json");


            return await client.PostAsync("api/v1/login", content);
        }
    }
}