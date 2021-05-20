using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SessionMaster.DAL.Entities
{
    public class SessionAnonymousUser
    {
        public Guid SessionId { get; set; }
        public Guid AnonymousUserId { get; set; }

        [ForeignKey("SessionId")]
        public Session Session { get; set; }

        [ForeignKey("AnonymousUserId")]
        public AnonymousUser AnonymousUser { get; set; }
    }
}