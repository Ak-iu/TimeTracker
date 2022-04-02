using System;
using System.Collections.Generic;
using System.Windows.Input;
using Storm.Mvvm;

namespace TimeTracker.Apps.Modeles
{
    public class Projet : NotifierBase
    {
        private string _nom;
        public string Nom
        {
            get => _nom;
            set => SetProperty(ref _nom, value);
        }

        private List<Tache> _taches;
        public List<Tache> Taches { 
            get => _taches; 
            set => SetProperty(ref _taches, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private string _id;

        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public ICommand DeleteCommand { get; set; }
   
        
        public ICommand SelectCommand { get; set; }

        public Projet(ICommand delete,ICommand select)
        {
            DeleteCommand = delete;
            SelectCommand = select;
            Taches = new List<Tache>();
        }

        public TimeSpan GetTotalTimes()
        {
            var totalTimes = TimeSpan.Zero;
            foreach (var t in _taches)
            {
                totalTimes = totalTimes.Add(t.GetTotalTimes());
            }
            return totalTimes;
        }
    }
}
