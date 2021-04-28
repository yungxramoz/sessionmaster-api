using System.Collections.Generic;

namespace SessionMaster.Common.Helpers
{
    public static class BoardGameAtlasFilterHelper
    {
        public static string ByMaxPlayer(string maxPlayer)
        {
            return $"&max_player={maxPlayer}";
        }
        public static string ByName(string name)
        {
            return $"&name={name}";
        }
        public static string ByIds(List<string> ids)
        {
            return $"&ids={string.Join(",", ids)}";
        }
    }
}
