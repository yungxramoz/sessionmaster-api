using SessionMaster.API.ModSession.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SessionMaster.API.ModSessionplan.ViewModels
{
    public class AddSessionplanModel
    {
        [Required]
        public string Name { get; set; }

        public List<SessionModel> Sessions { get; set; }

        public AddSessionplanModel()
        {
            Sessions = new List<SessionModel>();
        }
    }
}