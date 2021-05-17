using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SessionMaster.DAL.Entities
{
    public class AnonymousUser : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public ICollection<SessionAnonymousUser> SessionAnonymousUsers { get; set; }
    }
}