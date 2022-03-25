using System;
using System.Collections.Generic;
using System.Text;
using Storm.Mvvm;

namespace TimeTracker.Apps.Modeles
{
    public class Time : NotifierBase
    {
        private Guid _id;
        private DateTime _startTime;
        private DateTime _endTime;

        public Guid Id { get; set; }
        public DateTime StartTime { get => _startTime; set => SetProperty(ref _startTime, value); }
        public DateTime EndTime { get => _endTime; set => SetProperty(ref _endTime, value); }

        public Time()
        {
            _id = Guid.NewGuid();
        }

        public void StartTimer()
        {
            StartTime = DateTime.Now;
        }

        public void StopTimer()
        {
            EndTime = DateTime.Now;
        }
    }
}
