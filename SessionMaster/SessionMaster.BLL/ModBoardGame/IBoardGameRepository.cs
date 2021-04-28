using SessionMaster.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SessionMaster.BLL.ModBoardGame
{
    public interface IBoardGameRepository
    {
        Task<BoardGameAtlasGameDetails> GetById(string id, string clientId);

        Task<List<BoardGameAtlasGameDetails>> GetAll(string filter, string clientId);
    }
}