using System;

namespace SessionMaster.API.ModSessionplan.ViewModels
{
    public class SessionModel
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}