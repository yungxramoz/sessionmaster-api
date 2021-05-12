using AutoMapper;
using SessionMaster.API.ModSession.ViewModels;
using SessionMaster.DAL.Entities;

namespace SessionMaster.API.ModSession
{
    public class SessionMapProfile : Profile
    {
        public SessionMapProfile()
        {
            CreateMap<SessionModel, Session>().ReverseMap();
        }
    }
}