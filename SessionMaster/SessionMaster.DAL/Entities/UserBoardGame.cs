using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SessionMaster.DAL.Entities
{
    public class UserBoardGame : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(255)]
        public string BoardGameId { get; set; }

        [Required]
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}