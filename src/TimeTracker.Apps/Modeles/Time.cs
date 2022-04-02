using System;
using Storm.Mvvm;

namespace TimeTracker.Apps.Modeles
{
    public class Time : NotifierBase
    {
        private DateTime _startTime;
        private DateTime _endTime;
        public bool HasStarted;

        public DateTime StartTime { get => _startTime; set => SetProperty(ref _startTime, value); }
        public DateTime EndTime { get => _endTime; set => SetProperty(ref _endTime, value); }
        
        public string Id { get; set; }
        
        public void StartTimer()
        {
            var date = DateTime.Now;
            StartTime = date;
            HasStarted = true;
        }

        public void StopTimer()
        {
            EndTime = DateTime.Now;
            HasStarted = false;
        }
    }
}
