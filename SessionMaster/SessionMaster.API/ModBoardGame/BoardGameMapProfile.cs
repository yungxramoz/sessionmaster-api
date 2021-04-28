using AutoMapper;
using SessionMaster.API.ModBoardGame.ViewModels;
using SessionMaster.DAL.Entities;

namespace SessionMaster.API.ModUser
{
    public class BoardGameMapProfile : Profile
    {
        public BoardGameMapProfile()
        {
            CreateMap<BoardGameAtlasGameDetails, BoardGameModel>();
        }
    }
}