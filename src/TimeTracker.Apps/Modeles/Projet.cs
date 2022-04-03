using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using Storm.Mvvm;
using TimeTracker.Apps.Api;

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
        
        public TimeSpan TotalTime { get; set; }

        private List<Tache> _taches;

        public List<Tache> Taches
        {
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

        public Projet(ICommand delete, ICommand select)
        {
            DeleteCommand = delete;
            SelectCommand = select;
            Taches = new List<Tache>();
        }
        
        
    }
}