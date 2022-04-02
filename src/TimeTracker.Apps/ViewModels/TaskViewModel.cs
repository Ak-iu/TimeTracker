using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using TimeTracker.Apps.Api;
using TimeTracker.Apps.Modeles;
using TimeTracker.Apps.Pages;
using Xamarin.Forms;

namespace TimeTracker.Apps.ViewModels
{
    public class TaskViewModel : ViewModelBase
    {   
        //api
        public Projects projects = new Projects();
        public Authentication authentication = new Authentication();

        private GlobalVariables _global;
        
        private Projet projet;
        
        public TaskViewModel(Projet _projet)
        {
            _global = GlobalVariables.GetInstance();
            projet = _projet;
            Taches = new ObservableCollection<Tache>();
            //GetTasks();

            ProjectName = projet.Nom;
            ProjectDesc = projet.Description;
            
            AddCommand = new Command(AddTacheAction);
            ChartCommand = new Command(ChartAction);
        }

        private ObservableCollection<Tache> _taches;
        public ObservableCollection<Tache> Taches { get; set; }
        
        public ICommand AddCommand { get; set; }
        
        public ICommand ChartCommand { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDesc { get; set; }

        public void SelectAction(Tache tache)
        {
            /*Console.WriteLine("tache selectionne " + tache.Nom);
            await Application.Current.MainPage.DisplayAlert("Timer","Timer is on the task : " + tache.Nom,"OK");*/
            //TODO démarrer le timer ici + ajouté le temps depuis l'ouverture de l'app
            var navigationService = DependencyService.Get<INavigationService>();
            navigationService.PushAsync(new TimesPage(projet,tache));
        }

        public async void DeleteAction(Tache tache)
        {
            await projects.deleteTask(_global.AccessToken, projet.Id, tache.Id);
            await UpdateTokens(await authentication.Refresh(_global.RefreshToken));
            GetTasks();
        }
        
        private async void AddTacheAction()
        {
            string name =
                await Application.Current.MainPage.DisplayPromptAsync("New task", "task:");
            if (name != "")
            {
                await projects.addTask(_global.AccessToken, projet.Id, name);
                await UpdateTokens(await authentication.Refresh(_global.RefreshToken));
                GetTasks();
            }
        }

        public async void GetTasks()
        {
            Taches.Clear();
            HttpResponseMessage response = await projects.getTasks(_global.AccessToken,projet.Id);
            if (response != null && response.IsSuccessStatusCode)
            {
                JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());
                if ((bool) json.SelectToken("is_success"))
                {
                    JArray projectsList =
                        JArray.FromObject(json.SelectToken("data") ?? throw new InvalidOperationException());

                    foreach (var jToken in projectsList)
                    {
                        var id = jToken.SelectToken("id")?.ToString();
                        var name = jToken.SelectToken("name")?.ToString();
                        var timesList =
                            JArray.FromObject(jToken.SelectToken("times") ?? throw new InvalidOperationException());

                        var times = new List<Time>();
                        foreach (var jTime in timesList)
                        {
                            var tid = jTime.SelectToken("id")?.ToString();
                            var start = DateTime.Parse(jTime.SelectToken("start_time")?.ToString());
                            var end = DateTime.Parse(jTime.SelectToken("end_time")?.ToString());
                            var time = new Time()
                            {
                                StartTime = start, EndTime = end, Id = tid
                            };
                            times.Add(time);
                        }
                        Taches.Add(CreateTache(id,name,times));
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

        private Tache CreateTache(string id,string name,List<Time> times)
        {
            return new Tache(
                new Command<Tache>(DeleteAction),
                new Command<Tache>(SelectAction)
            )
            {
                Id = id,
                Nom = name,
                Times = times
            };
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
                }
            }
        }

        private void ChartAction()
        {
            INavigationService navigationService = DependencyService.Get<INavigationService>();
            navigationService.PushAsync(new ChartPage(projet.Nom,Taches));
        }
    }
}