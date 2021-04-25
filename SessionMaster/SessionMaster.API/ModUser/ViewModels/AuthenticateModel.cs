using System.ComponentModel.DataAnnotations;

namespace SessionMaster.API.ModUser.ViewModels
{
    public class AuthenticateModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}