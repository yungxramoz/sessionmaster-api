using System;

namespace SessionMaster.API.ModUser.ViewModels
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Token { get; set; }
    }
}
