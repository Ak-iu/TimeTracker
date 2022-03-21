using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using Storm.Mvvm.Services;
using TimeTracker.Apps.Pages;
using Xamarin.Forms;
using System;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using Xamarin.Forms;

namespace TimeTracker.Apps.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public ICommand _loginCommand;
        public ICommand _registerCommand;

        public String _email = "akiura";
        public String _firstName;
        public String _lastName;
        public String _password ="akiura";

        public String _error_code;
        public String _infos;


        public ICommand LoginCommand
        {
            get => _loginCommand;
        }

        public ICommand RegisterCommand
        {
            get => _registerCommand;
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public String ErrorCode
        {
            get => _error_code;
            set => SetProperty(ref _error_code, value);
        }

        public String Infos
        {
            get => _infos;
            set => SetProperty(ref _infos, value);
        }

        public LoginViewModel()
        {
            _registerCommand = new Command(RegisterAction);
            _loginCommand = new Command(LoginAction);
        }

        private async void LoginAction(object o)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://timetracker.julienmialon.ovh");

            JObject jsonData = new JObject(
                new JProperty("login", Email),
                new JProperty("password", Password),
                new JProperty("client_id", "MOBILE"),
                new JProperty("client_secret", "COURS"));
            var content = new StringContent(jsonData.ToString(), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("api/v1/login", content);

            if (response.IsSuccessStatusCode)
            {
                JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());
                if ((bool) json.SelectToken("is_success") == true)
                {
                    String accessToken = json.SelectToken("data")?.SelectToken("acces_token")?.ToString();
                    String refreshToken = json.SelectToken("data")?.SelectToken("refresh_token")?.ToString();

                    ErrorCode = "";
                    Infos = "Connected.";
                }
                else if ((bool) json.SelectToken("is_success") == false)
                {
                    ErrorCode = json.SelectToken("error_code").ToString();
                    Infos = "";
                }
            }
            else
            {
                ErrorCode = response.StatusCode.ToString();
                Infos = "";
            }

            
        }

        private async void RegisterAction(object o)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://timetracker.julienmialon.ovh");

            JObject jsonData = new JObject(
                new JProperty("client_id", "MOBILE"),
                new JProperty("client_secret", "COURS"),
                new JProperty("email", Email),
                new JProperty("first_name", FirstName),
                new JProperty("last_name", LastName),
                new JProperty("password", Password));

            var content = new StringContent(jsonData.ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("api/v1/register", content);

            if (response.IsSuccessStatusCode)
            {
                JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());
                if ((bool) json.SelectToken("is_success") == true)
                {
                    String accessToken = json.SelectToken("data")?.SelectToken("acces_token")?.ToString();
                    String refreshToken = json.SelectToken("data")?.SelectToken("refresh_token")?.ToString();

                    ErrorCode = "";
                    Infos = "Registration Completed.";
                }
                else if ((bool) json.SelectToken("is_success") == false)
                {
                    ErrorCode = json.SelectToken("error_code").ToString();
                    Infos = "";
                }
            }
            else
            {
                ErrorCode = response.StatusCode.ToString();
                Infos = "";
            }
        }
    }
}