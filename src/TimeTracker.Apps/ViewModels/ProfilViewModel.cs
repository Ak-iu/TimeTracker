using System;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using TimeTracker.Apps.Api;
using TimeTracker.Apps.Pages;
using Xamarin.Forms;

namespace TimeTracker.Apps.ViewModels
{
    public class ProfilViewModel : ViewModelBase
    {
        private User user = new User();
        private Authentication authentication = new Authentication();

        private string accessToken;
        private string refreshToken;

        private string _email;
        private string _firstName;
        private string _lastName;

        private ICommand _setEmail;
        private ICommand _setFirstName;
        private ICommand _setLastName;
        private ICommand _setPassword;

        public String _error_code;
        public String _infos;

        public ProfilViewModel(string _accessToken, string _refreshToken)
        {
            accessToken = _accessToken;
            refreshToken = _refreshToken;

            GetUserInfos();
            _setEmail = new Command(SetEmailAction);
            _setFirstName = new Command(SetFirstNameAction);
            _setLastName = new Command(SetLastNameAction);
            _setPassword = new Command(SetPasswordAction);
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

        public ICommand SetEmailCommand
        {
            get => _setEmail;
        }

        public ICommand SetFirstNameCommand
        {
            get => _setFirstName;
        }

        public ICommand SetLastNameCommand
        {
            get => _setLastName;
        }

        public ICommand SetPasswordCommand
        {
            get => _setPassword;
        }

        private async void SetEmailAction(object o)
        {
            string newEmail =
                await Application.Current.MainPage.DisplayPromptAsync("Edit your information", "new email address:");
            if (newEmail != "")
            {
                await user.Me(accessToken, newEmail, FirstName, LastName);
                await UpdateTokens(await authentication.Refresh(refreshToken));
                await GetUserInfos();
            }
        }

        private async void SetFirstNameAction()
        {
            String newFirstName =
                await Application.Current.MainPage.DisplayPromptAsync("Edit your information",
                    "new first name address:");
            if (newFirstName != "")
            {
                await user.Me(accessToken, Email, newFirstName, LastName);
                await UpdateTokens(await authentication.Refresh(refreshToken));
                await GetUserInfos();
            }
        }

        private async void SetLastNameAction()
        {
            string newLastName =
                await Application.Current.MainPage.DisplayPromptAsync("Edit your information",
                    "new last name address:");
            if (newLastName != "")
            {
                await user.Me(accessToken, Email, FirstName, newLastName);
                await UpdateTokens(await authentication.Refresh(refreshToken));
                await GetUserInfos();
            }
        }

        private async void SetPasswordAction()
        {
            string oldPassword =
                await Application.Current.MainPage.DisplayPromptAsync("Edit your information", "old password:");
            string newPassword =
                await Application.Current.MainPage.DisplayPromptAsync("Edit your information", "new password:");
            HttpResponseMessage response = await authentication.Password(accessToken, oldPassword, newPassword);

            JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (response.IsSuccessStatusCode)
            {
                if ((bool) json.SelectToken("is_success") == true)
                {
                    Infos = "Password modified.";
                    ErrorCode = "";
                }
                else if ((bool) json.SelectToken("is_success") == false)
                {
                    ErrorCode = json.SelectToken("error_code")?.ToString();
                    Infos = "";
                }
            }
        }

        public async Task GetUserInfos()
        {
            HttpResponseMessage response = await user.Me(accessToken);
            if (response.IsSuccessStatusCode)
            {
                JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());
                if ((bool) json.SelectToken("is_success") == true)
                {
                    Email = json.SelectToken("data")?.SelectToken("email")?.ToString();
                    FirstName = json.SelectToken("data")?.SelectToken("first_name")?.ToString();
                    LastName = json.SelectToken("data")?.SelectToken("last_name")?.ToString();
                }
            }
            else
            {
                ErrorCode = response.StatusCode.ToString();
            }
        }

        public async Task UpdateTokens(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());

                if ((bool) json.SelectToken("is_success"))
                {
                    accessToken = json.SelectToken("data")?.SelectToken("access_token")?.ToString();
                    refreshToken = json.SelectToken("data")?.SelectToken("refresh_token")?.ToString();
                    ErrorCode = "";
                    Infos = "User information has been modified.";
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