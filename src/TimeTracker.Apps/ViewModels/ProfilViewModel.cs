using System.Diagnostics;
using System.Net.Http;
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
        private string accessToken;
        private string refreshToken;

        private string _email;
        private string _firstName;
        private string _lastName;

        private ICommand _setPassword;

        public ProfilViewModel(string _accessToken, string _refreshToken)
        {
            accessToken = _accessToken;
            refreshToken = _refreshToken;

            GetUserInfos();
            _setPassword = new Command(SetPasswordAction);
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

        public ICommand SetPassword
        {
            get => _setPassword;
        }

        private void SetPasswordAction()
        {
            throw new System.NotImplementedException();
        }

        public async Task GetUserInfos()
        {
            User user = new User();
            
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
                Debug.WriteLine("Failed to get Profile");
            }
        }
    }
}