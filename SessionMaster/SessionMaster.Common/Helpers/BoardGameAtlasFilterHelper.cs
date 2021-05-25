using System.Collections.Generic;

namespace SessionMaster.Common.Helpers
{
    public static class BoardGameAtlasFilterHelper
    {
        public static string ByMaxPlayer(int? maxPlayer)
        {
            return maxPlayer != null ? $"&max_player={maxPlayer}" : "";
        }

        public static string ByMinPlayer(int? minPlayer)
        {
            return minPlayer != null ? $"&min_player={minPlayer}" : "";
        }

        public static string ByPlayerCount(int? playerCount)
        {
            if (playerCount != null)
            {
                var max = playerCount - 1;
                var min = playerCount + 1;
                return $"&lt_min_players={min}&gt_max_players={max}";
            }
            return "";
        }

        public static string ByName(string name)
        {
            return string.IsNullOrWhiteSpace(name) ? "" : $"&name={name}";
        }

        public static string ByIds(List<string> ids)
        {
            return $"&ids={string.Join(",", ids)}";
        }
    }
}