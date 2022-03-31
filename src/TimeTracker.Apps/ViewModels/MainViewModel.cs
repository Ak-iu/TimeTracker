using System.Windows.Input;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using TimeTracker.Apps.Pages;
using Xamarin.Forms;

namespace TimeTracker.Apps.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string accesToken;
        private string refreshToken;
        public ICommand _profilCommand;

        public ICommand ProfilCommand
        {
            get => _profilCommand;
        }

        public MainViewModel(string _accessToken,string _refreshToken)
        {
            accesToken = _accessToken;
            refreshToken = _refreshToken;
            _profilCommand = new Command(ProfilAction);
        }

        private void ProfilAction()
        {
            INavigationService navigationService = DependencyService.Get<INavigationService>();
            navigationService.PushAsync(new ProfilPage(accesToken, refreshToken));
        }
    }
}