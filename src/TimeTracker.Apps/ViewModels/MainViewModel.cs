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
    public class MainViewModel : ViewModelBase
    {
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

        public ICommand TimerCommand
        {
            get => _timerCommand;
            set => SetProperty(ref _timerCommand, value);
        }
        private ICommand _timerCommand;
        
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
            TimerCommand = new Command(StartTimerAction);
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
                await Projects.AddProject(_globals.AccessToken, name, description);
                await UpdateTokens(await Authentication.Refresh(_globals.RefreshToken));
                GetProjects();
            }
        }


        private async void DeleteAction(Projet projet)
        {
            Console.WriteLine("delete " + projet.Nom);
            await Projects.DeleteProject(_globals.AccessToken, projet.Id);
            await UpdateTokens(await Authentication.Refresh(_globals.RefreshToken));
            GetProjects();
        }

        private void SelectAction(Projet obj)
        {
            INavigationService navigationService = DependencyService.Get<INavigationService>();
            navigationService.PushAsync(new TaskPage(obj));
        }
        
        public async void GetProjects()
        {
            Projets.Clear();
            HttpResponseMessage response = await Projects.GetProjects(_globals.AccessToken);
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
            GetProjectsTasks();
        }
        
        public async void GetProjectsTasks()
        {
            foreach (var projet in _projets)
            {
                HttpResponseMessage response = await Projects.GetTasks(_globals.AccessToken,projet.Id);
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
                            projet.Taches.Add(CreateTache(id,name,times));
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
            
        }

        private Tache CreateTache(string id,string name,List<Time> times)
        {
            return new Tache()
            {
                Id = id,
                Nom = name,
                Times = times
            };
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
        
        private async void ChartAction()
        {
            await SetTotalTimes();
            var navigationService = DependencyService.Get<INavigationService>();
            navigationService.PushAsync(new ChartPage(Projets));
        }

        private void StartTimerAction()
        {
            _globals.GlobalTimer = new Time();
            _globals.GlobalTimer.StartTimer();
            var dialogService = DependencyService.Get<IDialogService>();
            dialogService.DisplayAlertAsync("Info", "You have started a new timer", "exit");
        }
        
        private async Task SetTotalTimes()
        {
            foreach (var project in Projets)
            {
                await SetProjectTotalTime(project);
            }
        }

        public async Task SetProjectTotalTime(Projet projet)
        {
            var totalTimes = TimeSpan.Zero;
    
            HttpResponseMessage response = await Projects.GetTasks(_globals.AccessToken, projet.Id);
            if (response != null && response.IsSuccessStatusCode)
            {
                JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());
                if ((bool) json.SelectToken("is_success"))
                {
                    JArray projectsList =
                        JArray.FromObject(json.SelectToken("data") ?? throw new InvalidOperationException());

                    foreach (var jToken in projectsList)
                    {
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
                        foreach (var t in times)
                        {
                            totalTimes = totalTimes.Add(t.EndTime.Subtract(t.StartTime));
                        }
                    }
                    projet.TotalTime = totalTimes;
                }
            }
        }
    }
}