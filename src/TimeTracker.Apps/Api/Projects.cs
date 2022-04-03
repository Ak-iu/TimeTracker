using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TimeTracker.Apps.Api
{
    public static class Projects
    {
        public static async Task<HttpResponseMessage> GetProjects(string accessToken)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://timetracker.julienmialon.ovh/api/v1/projects");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await client.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> DeleteProject(string accessToken, string id)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(new HttpMethod("DELETE"), "https://timetracker.julienmialon.ovh/api/v1/projects/"+id);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await client.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> AddProject(string accessToken,string name, string description)
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
        
        public static async Task<HttpResponseMessage> GetTasks(string accessToken,string projectId)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://timetracker.julienmialon.ovh/api/v1/projects/"+projectId+"/tasks");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await client.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> AddTask(string accessToken, string projectId, string name)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://timetracker.julienmialon.ovh/api/v1/projects/"+projectId+"/tasks");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            
            JObject jsonData = new JObject(
                new JProperty("name", name));
            var content = new StringContent(jsonData.ToString(), Encoding.UTF8, "application/json");
            request.Content = content;
            return await client.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> DeleteTask(string accessToken, string projectId, string taskId)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(new HttpMethod("DELETE"), "https://timetracker.julienmialon.ovh/api/v1/projects/"+projectId+"/tasks/"+taskId);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await client.SendAsync(request);
        }
        
        public static async Task<HttpResponseMessage> AddTime(string accessToken, string projectId, string taskId, DateTime startTime, DateTime endTime)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://timetracker.julienmialon.ovh/api/v1/projects/"+projectId+"/tasks/"+taskId+"/times");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var jsonData = new JObject()
            {
                new JProperty("start_time", startTime),
                new JProperty("end_time",endTime)
            };
            var content = new StringContent(jsonData.ToString(), Encoding.UTF8, "application/json");
            request.Content = content;
            return await client.SendAsync(request);
        }
        
        public static async Task<HttpResponseMessage> DeleteTime(string accessToken, string projectId, string taskId, string timeId)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(new HttpMethod("DELETE"), "https://timetracker.julienmialon.ovh/api/v1/projects/"+projectId+"/tasks/"+taskId+"/times/"+timeId);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await client.SendAsync(request);
        }
    }
}