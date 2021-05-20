using System;
using System.Collections.Generic;

namespace SessionMaster.API.ModSession.ViewModels
{
    public class SessionModel
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public IEnumerable<SessionUserModel> Users { get; set; }
    }
}