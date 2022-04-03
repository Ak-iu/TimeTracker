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

        public DateTime EndTime
        {
            get => _endTime;
            set
            {
                SetProperty(ref _endTime, value);
                SetText(_endTime.Subtract(_startTime));
            }
        }

        public string DurationText { get; set; }
        
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

        private void SetText(TimeSpan span)
        {
            DurationText = (span.Days * 24 + span.Hours) + ":" + span.Minutes.ToString("00") + ":" + span.Seconds.ToString("00");
        }
    }
}
