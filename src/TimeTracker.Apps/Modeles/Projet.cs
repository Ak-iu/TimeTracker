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
        public ICommand AddTacheCommand { get; set; }

        public Projet(ICommand delete, ICommand addTache)
        {
            DeleteCommand = delete;
            AddTacheCommand = addTache;
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

        private Command selectCommand;

        public ICommand SelectCommand
        {
            get
            {
                if (selectCommand == null)
                {
                    selectCommand = new Command(Select);
                }

                return selectCommand;
            }
        }

        private void Select()
        {
        }
    }
}
