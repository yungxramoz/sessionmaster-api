using AutoMapper;
using SessionMaster.API.ModSession.ViewModels;
using SessionMaster.DAL.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SessionMaster.API.ModSession
{
    public class SessionMapProfile : Profile
    {
        public SessionMapProfile()
        {
            CreateMap<Session, SessionModel>().ReverseMap();
        }
    }
}