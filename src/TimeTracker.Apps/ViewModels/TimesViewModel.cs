using System.Collections.ObjectModel;
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
    public class TimesViewModel : ViewModelBase
    {
        private GlobalVariables _global;
        
        private readonly Projet _projet;
        private readonly Tache _tache;
        
        public string TaskName { get => _taskname; set => SetProperty(ref _taskname,value); }
        private string _taskname;

        public ObservableCollection<Time> Times { get => _times; set => SetProperty(ref _times,value); }
        private ObservableCollection<Time> _times;

        public ICommand TimerCommand { get => _timerCommand; set => SetProperty(ref _timerCommand,value); }
        private ICommand _timerCommand;

        public string TimerButtonText { get => _timerText; set => SetProperty(ref _timerText,value); }
        private string _timerText;

        public TimesViewModel(Projet projet, Tache tache)
        {
            _global = GlobalVariables.GetInstance();
            _projet = projet;
            _tache = tache;
            
            Times = new ObservableCollection<Time>(tache.Times);
            
            TaskName = tache.Nom;

            if (_global.GlobalTimer == null || ! _global.GlobalTimer.HasStarted)
            {
                TimerCommand = new Command(StartTimerAction);
                TimerButtonText = "Start Timer";
            }
            else
            {
                TimerCommand = new Command(StopTimerAction);
                TimerButtonText = "Stop Timer";
            }
        }

        private void StartTimerAction()
        {
            _global.GlobalTimer = new Time();
            _global.GlobalTimer.StartTimer();
            TimerButtonText = "Stop Timer";
            TimerCommand = new Command(StopTimerAction);
        }

        private async void StopTimerAction()
        {
            _global.GlobalTimer.StopTimer();
            TimerButtonText = "Start Timer";
            TimerCommand = new Command(StartTimerAction);

            await Projects.AddTime(_global.AccessToken, _projet.Id, _tache.Id, _global.GlobalTimer.StartTime , _global.GlobalTimer.EndTime);
            await UpdateTokens(await Authentication.Refresh(_global.RefreshToken));
            _tache.Times.Add(_global.GlobalTimer);
            Times.Add(_global.GlobalTimer);
        }
        
        private async Task UpdateTokens(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var json = JObject.Parse(await response.Content.ReadAsStringAsync());

                if ((bool) json.SelectToken("is_success"))
                {
                    _global.AccessToken = json.SelectToken("data")?.SelectToken("access_token")?.ToString();
                    _global.RefreshToken = json.SelectToken("data")?.SelectToken("refresh_token")?.ToString();
                }
            }
        }
    }
}