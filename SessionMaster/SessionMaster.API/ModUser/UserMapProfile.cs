using AutoMapper;
using SessionMaster.API.ModUser.ViewModels;
using SessionMaster.DAL.Entities;

namespace SessionMaster.API.ModUser
{
    public class UserMapProfile : Profile
    {
        public UserMapProfile()
        {
            CreateMap<User, UserModel>();
        }
    }
}
