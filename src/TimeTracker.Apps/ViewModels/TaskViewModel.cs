using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using Storm.Mvvm;
using TimeTracker.Apps.Api;
using TimeTracker.Apps.Modele;
using TimeTracker.Apps.Modeles;
using Xamarin.Forms;

namespace TimeTracker.Apps.ViewModels
{
    public class TaskViewModel : ViewModelBase
    {   
        //api
        public Projects projects = new Projects();
        public Authentication authentication = new Authentication();
        
        private string accessToken;
        private string refreshToken;
        private Projet projet;
        public TaskViewModel(string _accessToken, string _refreshToken,Projet _projet)
        {
            accessToken = _accessToken;
            refreshToken = _refreshToken;
            projet = _projet;
            Taches = new ObservableCollection<Tache>();
            GetTasks();

            ProjectName = projet.Nom;
            ProjectDesc = projet.Description;
            
            AddCommand = new Command(AddTacheAction);
        }

        private ObservableCollection<Tache> _taches;
        public ObservableCollection<Tache> Taches { get; set; }
        
        public ICommand AddCommand { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDesc { get; set; }

        public async void SelectAction(Tache tache)
        {
            Console.WriteLine("tache selectionne " + tache.Nom);
            await Application.Current.MainPage.DisplayAlert("Timer","Timer is on the task :" + tache.Nom,"OK");
            //TODO start timer here
        }

        public async void DeleteAction(Tache tache)
        {
            await projects.deleteTask(accessToken, projet.Id, tache.Id);
            await UpdateTokens(await authentication.Refresh(refreshToken));
            GetTasks();
        }
        
        private async void AddTacheAction()
        {
            string name =
                await Application.Current.MainPage.DisplayPromptAsync("New task", "task:");
            if (name != "")
            {
                await projects.addTask(accessToken, projet.Id, name);
                await UpdateTokens(await authentication.Refresh(refreshToken));
                GetTasks();
            }
        }

        private async void GetTasks()
        {
            Taches.Clear();
            HttpResponseMessage response = await projects.getTasks(accessToken,projet.Id);
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
                        Taches.Add(CreateTache(id,name));
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

        private Tache CreateTache(string id,string name)
        {
            return new Tache(
                new Command<Tache>(DeleteAction),
                new Command<Tache>(SelectAction)
            )
            {
                Id = id,
                Nom = name,
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