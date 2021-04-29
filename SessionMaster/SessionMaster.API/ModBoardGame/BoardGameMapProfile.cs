using AutoMapper;
using SessionMaster.API.ModBoardGame.ViewModels;
using SessionMaster.DAL.Entities;
using System;

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