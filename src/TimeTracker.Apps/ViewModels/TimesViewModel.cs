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
        private readonly Projects _projects = new Projects();
        private readonly Authentication _authentication = new Authentication();

        private GlobalVariables _global;
        
        private Time _time;
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
            TimerCommand = new Command(StartTimerAction);
            
            TaskName = tache.Nom;
            TimerButtonText = "Start Timer";
        }

        private void StartTimerAction()
        {
            _time = new Time();
            _time.StartTimer();
            TimerButtonText = "Stop Timer";
            TimerCommand = new Command(StopTimerAction);
        }

        private async void StopTimerAction()
        {
            _time.StopTimer();
            TimerButtonText = "Start Timer";
            TimerCommand = new Command(StartTimerAction);

            var addTime = await _projects.AddTime(_global.AccessToken, _projet.Id, _tache.Id, _time.StartTime , _time.EndTime);
            await UpdateTokens(await _authentication.Refresh(_global.RefreshToken));
            _tache.Times.Add(_time);
            Times.Add(_time);
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