using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;


namespace TimeTracker.Apps.Api
{
    public class User
    {
        public async Task<HttpResponseMessage> Me(string accessToken)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://timetracker.julienmialon.ovh/api/v1/me");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await client.SendAsync(request);
        }

        public async Task<HttpResponseMessage> Me(string accessToken, string email, string firsName, string lastName)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), "https://timetracker.julienmialon.ovh/api/v1/me");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            JObject jsonData = new JObject(
                new JProperty("email", email),
                new JProperty("first_name", firsName),
                new JProperty("last_name", lastName));
            var content = new StringContent(jsonData.ToString(), Encoding.UTF8, "application/json");
            request.Content = content;
            
            return await client.SendAsync(request);
        }
    }
}