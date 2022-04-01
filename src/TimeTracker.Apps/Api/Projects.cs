using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

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

        public async Task<HttpResponseMessage> addProject(string accessToken,string name, string description)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://timetracker.julienmialon.ovh/api/v1/projects");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            
            JObject jsonData = new JObject(
                new JProperty("name", name),
                new JProperty("description", description));
            var content = new StringContent(jsonData.ToString(), Encoding.UTF8, "application/json");
            request.Content = content;
            return await client.SendAsync(request);
        }
        
        public async Task<HttpResponseMessage> getTasks(string accessToken,string projectId)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://timetracker.julienmialon.ovh/api/v1/projects/"+projectId+"/tasks");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await client.SendAsync(request);
        }

        public async Task<HttpResponseMessage> addTask(string accessToken, string projectID, string name)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://timetracker.julienmialon.ovh/api/v1/projects/"+projectID+"/tasks");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            
            JObject jsonData = new JObject(
                new JProperty("name", name));
            var content = new StringContent(jsonData.ToString(), Encoding.UTF8, "application/json");
            request.Content = content;
            return await client.SendAsync(request);
        }

        public async Task<HttpResponseMessage> deleteTask(string accessToken, string projectId, string taskId)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(new HttpMethod("DELETE"), "https://timetracker.julienmialon.ovh/api/v1/projects/"+projectId+"/tasks/"+taskId);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await client.SendAsync(request);
        }
    }
}