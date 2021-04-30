using System.Text.Json.Serialization;

namespace SessionMaster.DAL.Entities
{
    public class BoardGameAtlasGameDetails
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("year_published")]
        public int? PublishYear { get; set; }

        [JsonPropertyName("min_players")]
        public int? MinPlayers { get; set; }

        [JsonPropertyName("max_players")]
        public int? MaxPlayers { get; set; }

        [JsonPropertyName("min_playtime")]
        public int? MinPlaytime { get; set; }

        [JsonPropertyName("max_playtime")]
        public int? MaxPlaytime { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("thumb_url")]
        public string ThumbUrl { get; set; }

        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; }
    }
}