using System;
using System.ComponentModel.DataAnnotations;

namespace SessionMaster.DAL.Entities
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
