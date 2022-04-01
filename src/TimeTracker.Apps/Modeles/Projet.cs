using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Storm.Mvvm;
using TimeTracker.Apps.Modeles;
using Xamarin.Forms;

namespace TimeTracker.Apps.Modele
{
    public class Projet : NotifierBase
    {
        private string _nom;
        public string Nom
        {
            get;
            set;
        }

        private List<Tache> _taches;
        public List<Tache> Taches { 
            get => _taches; 
            set => SetProperty(ref _taches, value);
        }

        private string _description;
        public string Description
        {
            get;
            set;
        }

        private string _id;
        public string Id { get; set; }

        public ICommand DeleteCommand { get; set; }
   
        
        public ICommand SelectCommand { get; set; }

        public Projet(ICommand delete,ICommand select)
        {
            DeleteCommand = delete;
            SelectCommand = select;
        }

        public TimeSpan getTotalTimes()
        {
            TimeSpan totalTimes = TimeSpan.Zero;
            foreach (Tache t in _taches)
            {
                totalTimes.Add(t.GetTotalTimes());
            }
            return totalTimes;
        }
        
        private void Select()
        {
        }
    }
}
