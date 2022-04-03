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
        private GlobalVariables _global;
        
        private Projet _projet;
        
        public TaskViewModel(Projet projet)
        {
            _global = GlobalVariables.GetInstance();
            this._projet = projet;
            Taches = new ObservableCollection<Tache>();
            //GetTasks();

            ProjectName = _projet.Nom;
            ProjectDesc = _projet.Description;
            
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
            var navigationService = DependencyService.Get<INavigationService>();
            navigationService.PushAsync(new TimesPage(_projet,tache));
        }

        public async void DeleteAction(Tache tache)
        {
            await Projects.DeleteTask(_global.AccessToken, _projet.Id, tache.Id);
            await UpdateTokens(await Authentication.Refresh(_global.RefreshToken));
            GetTasks();
        }
        
        private async void AddTacheAction()
        {
            string name =
                await Application.Current.MainPage.DisplayPromptAsync("New task", "task:");
            if (name != "")
            {
                await Projects.AddTask(_global.AccessToken, _projet.Id, name);
                await UpdateTokens(await Authentication.Refresh(_global.RefreshToken));
                GetTasks();
            }
        }

        public async void GetTasks()
        {
            Taches.Clear();
            HttpResponseMessage response = await Projects.GetTasks(_global.AccessToken,_projet.Id);
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
            navigationService.PushAsync(new ChartPage(_projet.Nom,Taches));
        }
    }
}