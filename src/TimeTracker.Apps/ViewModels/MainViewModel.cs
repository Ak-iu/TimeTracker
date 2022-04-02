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
using TimeTracker.Apps.Modeles;
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

        private GlobalVariables _globals;

        private ObservableCollection<Projet> _projets;

        public ObservableCollection<Projet> Projets
        {
            get => _projets;
            set => SetProperty(ref _projets, value);
        }
        public ICommand ProfilCommand { get; set; }

        public ICommand AddCommand { get; set; }
        
        public ICommand ChartCommand { get; set; }

        public MainViewModel(string accessToken, string refreshToken)
        {
            _globals = GlobalVariables.GetInstance();
            _globals.AccessToken = accessToken;
            _globals.RefreshToken = refreshToken;
            Projets = new ObservableCollection<Projet>();
            //GetProjects();

            ProfilCommand = new Command(ProfilAction);
            AddCommand = new Command(AddProjectAction);
            ChartCommand = new Command(ChartAction);
        }

        private void ProfilAction()
        {
            INavigationService navigationService = DependencyService.Get<INavigationService>();
            navigationService.PushAsync(new ProfilPage());
        }

        private async void AddProjectAction()
        {
            string name =
                await Application.Current.MainPage.DisplayPromptAsync("New project", "Project's name:");
            string description =
                await Application.Current.MainPage.DisplayPromptAsync("New project", "Project's description:");

            if (name != "" && description != "")
            {
                await projects.addProject(_globals.AccessToken, name, description);
                await UpdateTokens(await authentication.Refresh(_globals.RefreshToken));
                GetProjects();
            }
        }


        private async void DeleteAction(Projet projet)
        {
            Console.WriteLine("delete " + projet.Nom);
            await projects.deleteProject(_globals.AccessToken, projet.Id);
            await UpdateTokens(await authentication.Refresh(_globals.RefreshToken));
            GetProjects();
        }

        private void SelectAction(Projet obj)
        {
            INavigationService navigationService = DependencyService.Get<INavigationService>();
            navigationService.PushAsync(new TaskPage(obj));
        }
        
        private async void GetProjects()
        {
            Projets.Clear();
            HttpResponseMessage response = await projects.getProjects(_globals.AccessToken);
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

        private Projet CreateProject(string id, string name, string desc)
        {
            return new Projet(
                new Command<Projet>(DeleteAction),
                new Command<Projet>(SelectAction)
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
                    _globals.AccessToken = json.SelectToken("data")?.SelectToken("access_token")?.ToString();
                    _globals.RefreshToken = json.SelectToken("data")?.SelectToken("refresh_token")?.ToString();
                }
            }
        }
        
        private void ChartAction()
        {
            var navigationService = DependencyService.Get<INavigationService>();
            navigationService.PushAsync(new ChartPage(Projets));
        }
    }
}