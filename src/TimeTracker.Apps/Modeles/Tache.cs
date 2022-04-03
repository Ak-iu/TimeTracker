using System;
using System.Collections.Generic;
using Storm.Mvvm;
using System.Windows.Input;

namespace TimeTracker.Apps.Modeles
{
    public class Tache : NotifierBase
    {
        public string Nom { get; set; }
        public string Id { get; set; }
        public List<Time> Times { get ; set; }
        
        public ICommand SelectCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        
        public Tache(ICommand delete,ICommand select)
        {
            DeleteCommand = delete;
            SelectCommand = select;
            Times = new List<Time>();
        }

        public Tache()
        {
            Times = new List<Time>();
        }
        
        public TimeSpan GetTotalTimes()
        {
            var totalTimes = TimeSpan.Zero;
            foreach (var t in Times)
            {
                totalTimes = totalTimes.Add(t.EndTime.Subtract(t.StartTime));
            }
            return totalTimes;
        }
    }
}
