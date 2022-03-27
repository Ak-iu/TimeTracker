using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.Apps.Api
{
    public class User
    {
        public async Task<HttpResponseMessage> Me(string accessToken)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://timetracker.julienmialon.ovh/api/v1/me");
            Debug.WriteLine(accessToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await client.SendAsync(request);
     
        }
            
    }
}