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

        public async Task<HttpResponseMessage> deleteProject(string accessToken, string id)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(new HttpMethod("DELETE"), "https://timetracker.julienmialon.ovh/api/v1/projects/"+id);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await client.SendAsync(request);
        }
    }
}