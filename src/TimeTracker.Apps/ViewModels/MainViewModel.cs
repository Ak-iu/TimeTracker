using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using TimeTracker.Apps.Modele;
using TimeTracker.Apps.Pages;
using Xamarin.Forms;

namespace TimeTracker.Apps.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string accesToken;
        private string refreshToken;

        private ObservableCollection<Projet> _projets;
        
        
        public ICommand _profilCommand;
        public ICommand _addCommand;
        
        
        public ObservableCollection<Projet> Projets
        {
            get => _projets;
            set => SetProperty(ref _projets, value);
        }
        public ICommand ProfilCommand
        {
            get => _profilCommand;
            set => SetProperty(ref _profilCommand, value);
        }

        public ICommand AddCommand
        {
            get => _addCommand;
            set => SetProperty(ref _addCommand, value);
        }

        public MainViewModel(string _accessToken,string _refreshToken)
        {
            accesToken = _accessToken;
            refreshToken = _refreshToken;

            Projets = new ObservableCollection<Projet>();
            //TODO debug test projet
            
            Projets.Add(new Projet("0","nom","desc",_profilCommand,_profilCommand) );
            Projets.Add(new Projet("1","nom2","desc2",_profilCommand,_profilCommand) );

            
            ProfilCommand = new Command(ProfilAction);
        }

        private void ProfilAction()
        {
            INavigationService navigationService = DependencyService.Get<INavigationService>();
            navigationService.PushAsync(new ProfilPage(accesToken, refreshToken));
        }
    }
}