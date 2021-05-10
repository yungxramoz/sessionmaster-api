using AutoMapper;
using SessionMaster.API.ModSessionplan.ViewModels;
using SessionMaster.DAL.Entities;

namespace SessionMaster.API.ModSessionplan
{
    public class SessionplanMapProfile : Profile
    {
        public SessionplanMapProfile()
        {
            CreateMap<Sessionplan, SessionplanOverviewModel>();
            CreateMap<Sessionplan, SessionplanDetailModel>();
            CreateMap<AddSessionplanModel, Sessionplan>();
            CreateMap<UpdateSessionplanModel, Sessionplan>();
        }
    }
}