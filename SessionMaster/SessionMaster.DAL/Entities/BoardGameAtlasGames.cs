using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SessionMaster.DAL.Entities
{
    public class BoardGameAtlasGames
    {
        [JsonPropertyName("games")]
        public List<BoardGameAtlasGameDetails> Games { get; set; }
    }
}