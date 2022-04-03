using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using Storm.Mvvm;
using TimeTracker.Apps.Api;
using TimeTracker.Apps.Modeles;
using Xamarin.Forms;

namespace TimeTracker.Apps.ViewModels
{
    public class ProfilViewModel : ViewModelBase
    {
        private GlobalVariables _global;

        private string _email;
        private string _firstName;
        private string _lastName;

        private ICommand _setEmail;
        private ICommand _setFirstName;
        private ICommand _setLastName;
        private ICommand _setPassword;

        private string _errorCode;
        private string _infos;

        public ProfilViewModel()
        {
            _global = GlobalVariables.GetInstance();

            GetUserInfos();
            _setEmail = new Command(SetEmailAction);
            _setFirstName = new Command(SetFirstNameAction);
            _setLastName = new Command(SetLastNameAction);
            _setPassword = new Command(SetPasswordAction);
        }

        public string ErrorCode
        {
            get => _errorCode;
            set => SetProperty(ref _errorCode, value);
        }

        public string Infos
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
                await User.Me(_global.AccessToken, newEmail, FirstName, LastName);
                await UpdateTokens(await Authentication.Refresh(_global.RefreshToken));
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
                await User.Me(_global.AccessToken, Email, newFirstName, LastName);
                await UpdateTokens(await Authentication.Refresh(_global.RefreshToken));
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
                await User.Me(_global.AccessToken, Email, FirstName, newLastName);
                await UpdateTokens(await Authentication.Refresh(_global.RefreshToken));
                await GetUserInfos();
            }
        }

        private async void SetPasswordAction()
        {
            string oldPassword =
                await Application.Current.MainPage.DisplayPromptAsync("Edit your information", "old password:");
            string newPassword =
                await Application.Current.MainPage.DisplayPromptAsync("Edit your information", "new password:");
            HttpResponseMessage response = await Authentication.Password(_global.AccessToken, oldPassword, newPassword);

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
            HttpResponseMessage response = await User.Me(_global.AccessToken);
            if (response.IsSuccessStatusCode)
            {
                JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());
                if ((bool) json.SelectToken("is_success"))
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
                    _global.AccessToken = json.SelectToken("data")?.SelectToken("access_token")?.ToString();
                    _global.RefreshToken = json.SelectToken("data")?.SelectToken("refresh_token")?.ToString();
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