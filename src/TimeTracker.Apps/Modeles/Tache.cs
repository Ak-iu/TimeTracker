using System;
using System.Collections.Generic;
using Storm.Mvvm;
using System.Text;

namespace TimeTracker.Apps.Modeles
{
    public class Tache : NotifierBase
    {
        private string _nom;
        private Guid _id;
        private List<Time> _times;

        public string Nom { 
            get => _nom;
            set => SetProperty(ref _nom, value);
        }
        public Guid Id { get; }
        public List<Time> Times {
            get => _times;
            set => SetProperty(ref _times, value);
        }

        public Tache(string nom)
        {
            _nom = nom;
        }
        
        public TimeSpan GetTotalTimes()
        {
            TimeSpan totalTimes = TimeSpan.Zero;
            foreach (Time t in _times)
            {
                totalTimes.Add(t.EndTime.Subtract(t.StartTime));
            }
            return totalTimes;
        }
    }
}
