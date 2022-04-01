using System;
using System.Collections.Generic;
using Storm.Mvvm;
using System.Text;
using System.Windows.Input;

namespace TimeTracker.Apps.Modeles
{
    public class Tache : NotifierBase
    {
        private string _nom;
        private string _id;
        private List<Time> _times;

        public string Nom { get; set; }
        public string Id { get; set; }
        public List<Time> Times { get; set; }
        
        public ICommand SelectCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        
        public Tache(ICommand delete,ICommand select)
        {
            DeleteCommand = delete;
            SelectCommand = select;
            Times = new List<Time>();
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
