using AutoMapper;
using SessionMaster.API.ModUser.ViewModels;
using SessionMaster.DAL.Entities;
using UserAuthentication.Models;

namespace SessionMaster.API.ModUser
{
    public class UserMapProfile : Profile
    {
        public UserMapProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<RegistrationModel, User>();
            CreateMap<UpdateUserModel, User>();
        }
    }
}
