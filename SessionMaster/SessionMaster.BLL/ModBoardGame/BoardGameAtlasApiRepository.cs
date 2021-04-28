using RestSharp;
using SessionMaster.Common.Exceptions;
using SessionMaster.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace SessionMaster.BLL.ModBoardGame
{
    public class BoardGameAtlasApiRepository : IBoardGameRepository
    {
        public const string BASE_URL = "https://api.boardgameatlas.com/api";
        public const string FIELDS = "&fields=id,name,year_published,min_players,max_players,min_playtime,max_playtime,description,thumb_url,image_url";

        public async Task<BoardGameAtlasGameDetails> GetById(string id, string clientId)
        {
            var client = new RestClient($"{BASE_URL}/search?client_id={clientId}{FIELDS}&ids={id}");
            var request = new RestRequest(Method.GET);

            try
            {
                IRestResponse response = await client.ExecuteAsync(request);
                if (response.IsSuccessful)
                {
                    var boardGameInfo = JsonSerializer.Deserialize<BoardGameAtlasGames>(response.Content);

                    return boardGameInfo.Games.FirstOrDefault();
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new NotFoundException("Board game not found");
                }
                else
                {
                    throw new InfoException(response.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                throw new InfoException(ex.Message);
            }
        }

        public async Task<List<BoardGameAtlasGameDetails>> GetAll(string filter, string clientId)
        {
            var client = new RestClient($"{BASE_URL}/search?client_id={clientId}{FIELDS}{filter}&order_by=popularity&limit=100");
            var request = new RestRequest(Method.GET);

            try
            {
                IRestResponse response = await client.ExecuteAsync(request);
                if (response.IsSuccessful)
                {
                    var boardGameInfo = JsonSerializer.Deserialize<BoardGameAtlasGames>(response.Content);

                    return boardGameInfo.Games;
                }
                else
                {
                    throw new InfoException("An unexpected error occured");
                }
            }
            catch (Exception ex)
            {
                throw new InfoException(ex.Message);
            }
        }
    }
}