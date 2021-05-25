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
                var greaterThenMin = playerCount - 1;
                var lesserThenMax = playerCount + 1;
                return $"&gt_min_players={greaterThenMin}&lt_max_players={lesserThenMax}";
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