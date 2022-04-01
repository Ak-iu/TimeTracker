using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TimeTracker.Apps.Api
{
    public class Projects
    {
        public async Task<HttpResponseMessage> getProjects(string accessToken)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://timetracker.julienmialon.ovh/api/v1/projects");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await client.SendAsync(request);
        }
    }
}