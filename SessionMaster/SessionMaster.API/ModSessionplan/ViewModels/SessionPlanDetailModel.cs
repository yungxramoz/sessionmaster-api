using System;
using System.Collections.Generic;

namespace SessionMaster.API.ModSessionplan.ViewModels
{
    public class SessionPlanDetailModel : SessionPlanOverviewModel
    {
        public List<SessionModel> Sessions { get; set; }
    }
}