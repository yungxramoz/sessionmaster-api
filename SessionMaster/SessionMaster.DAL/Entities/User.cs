using System.ComponentModel.DataAnnotations;

namespace SessionMaster.DAL.Entities
{
    public class User : BaseEntity
    {
        [StringLength(255)]
        public string Username { get; set; }

        [StringLength(255)]
        public string Firstname { get; set; }

        [StringLength(255)]
        public string Lastname { get; set; }

        [StringLength(64)]
        public byte[] PasswordHash { get; set; }

        [StringLength(128)]
        public byte[] PasswordSalt { get; set; }
    }
}
