using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using TimeTracker.Apps.Api;
using TimeTracker.Apps.Modele;
using TimeTracker.Apps.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace TimeTracker.Apps.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        //api
        private Projects projects = new Projects();
        private Authentication authentication = new Authentication();

        private string accessToken;
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
            get;
            set;
        }

        public MainViewModel(string _accessToken, string _refreshToken)
        {
            accessToken = _accessToken;
            refreshToken = _refreshToken;
            Projets = new ObservableCollection<Projet>();
            GetProjects();

            ProfilCommand = new Command(ProfilAction);
            AddCommand = new Command(AddProjectAction);
        }

        private void ProfilAction()
        {
            INavigationService navigationService = DependencyService.Get<INavigationService>();
            navigationService.PushAsync(new ProfilPage(accessToken, refreshToken));
        }


        private async void AddProjectAction()
        {
            
        }
        
        private async void DeleteAction(Projet projet)
        {
            Console.WriteLine("delete " + projet.Nom);
            await projects.deleteProject(accessToken, projet.Id);
            await UpdateTokens(await authentication.Refresh(refreshToken));
            GetProjects();
        }
        private void AddTacheAction(Projet obj)
        {
           
        }
        
        
        
        
        
        
        private async void GetProjects()
        {   
            Projets.Clear();
            HttpResponseMessage response = await projects.getProjects(accessToken);
            if (response != null && response.IsSuccessStatusCode)
            {
                JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());
                if ((bool) json.SelectToken("is_success"))
                {
                    JArray projectsList =
                        JArray.FromObject(json.SelectToken("data") ?? throw new InvalidOperationException());

                    foreach (var jToken in projectsList)
                    {
                        string id = jToken.SelectToken("id")?.ToString();
                        string name = jToken.SelectToken("name")?.ToString();
                        string desc = jToken.SelectToken("description")?.ToString();
                        Projets.Add(CreateProject(id, name, desc));
                    }
                }
                else if ((bool) json.SelectToken("is_success") == false)
                {
                    Console.WriteLine(json.SelectToken("error_code")?.ToString());
                }
            }
            else
            {
                Console.WriteLine(response.StatusCode.ToString());
            }
        }

        private Projet CreateProject(string id,string name,string desc)
        {
            return new Projet(
                new Command<Projet>(DeleteAction),
                new Command<Projet>(AddTacheAction)
            )
            {
                Id = id,
                Nom = name,
                Description = desc
            };
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
                }
            }
        }
    }
}