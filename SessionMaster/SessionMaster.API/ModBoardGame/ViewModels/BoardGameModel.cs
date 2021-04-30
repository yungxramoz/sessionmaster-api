namespace SessionMaster.API.ModBoardGame.ViewModels
{
    public class BoardGameModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int? PublishYear { get; set; }
        public int? MinPlayers { get; set; }
        public int? MaxPlayers { get; set; }
        public int? MinPlaytime { get; set; }
        public int? MaxPlaytime { get; set; }
        public string Description { get; set; }
        public string ThumbUrl { get; set; }
        public string ImageUrl { get; set; }
    }
}