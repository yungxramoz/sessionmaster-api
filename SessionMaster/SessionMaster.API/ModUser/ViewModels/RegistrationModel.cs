using System.ComponentModel.DataAnnotations;

namespace SessionMaster.API.ModUser.ViewModels
{
    public class RegistrationModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Firstname { get; set; }

        [Required]
        public string Lastname { get; set; }
    }
}