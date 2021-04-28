using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SessionMaster.DAL.Entities
{
    public class BoardGameAtlasGames
    {
        [JsonPropertyName("games")]
        public List<BoardGameAtlasGameDetails> Games { get; set; }
    }
}
