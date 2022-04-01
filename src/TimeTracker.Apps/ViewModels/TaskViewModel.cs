using Storm.Mvvm;
using TimeTracker.Apps.Modele;

namespace TimeTracker.Apps.ViewModels
{
    public class TaskViewModel : ViewModelBase
    {
        private string accessToken;
        private string refreshToken;
        private Projet projet;
        public TaskViewModel(string _accessToken, string _refreshToken,Projet _projet)
        {
            accessToken = _accessToken;
            refreshToken = _refreshToken;
            projet = _projet;

            ProjectName = projet.Nom;
            ProjectDesc = projet.Description;
        }

        public string ProjectName { get; set; }
        public string ProjectDesc { get; set; }
    }
}