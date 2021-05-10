using System;
using System.Collections.Generic;

namespace SessionMaster.API.ModSessionplan.ViewModels
{
    public class SessionplanDetailModel : SessionplanOverviewModel
    {
        public List<SessionModel> Sessions { get; set; }
    }
}