﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SessionMaster.DAL.Entities
{
    public class Session : BaseEntity
    {
        [Required]
        public Guid SessionplanId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [Required]
        [ForeignKey("SessionplanId")]
        public Sessionplan Sessionplan { get; set; }
    }
}