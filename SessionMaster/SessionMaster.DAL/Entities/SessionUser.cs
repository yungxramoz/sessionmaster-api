using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SessionMaster.DAL.Entities
{
    public class SessionUser
    {
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }

        [ForeignKey("SessionId")]
        public Session Session { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}