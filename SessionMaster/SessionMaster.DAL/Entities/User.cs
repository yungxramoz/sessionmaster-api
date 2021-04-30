using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SessionMaster.DAL.Entities
{
    public class User : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string Username { get; set; }

        [Required]
        [StringLength(255)]
        public string Firstname { get; set; }

        [Required]
        [StringLength(255)]
        public string Lastname { get; set; }

        [Required]
        [StringLength(64)]
        public byte[] PasswordHash { get; set; }

        [Required]
        [StringLength(128)]
        public byte[] PasswordSalt { get; set; }

        public ICollection<UserBoardGame> BoardGames { get; set; }
    }
}