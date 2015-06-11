﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Realm.DAL.Models
{
    [Table("Behaviors")]
    public class Behavior : IPrimitive
    {
        [Key]
        public int Id { get; set; }

        public DateTime? CreateDateUtc { get; set; }

        [Required]
        [MaxLength(255), MinLength(5)]
        public string SystemName { get; set; }

        [Required]
        public int? SystemClassId { get; set; }
        public virtual SystemClass SystemClass { get; set; }

        [Required]
        [MaxLength(255), MinLength(5)]
        public string DisplayName { get; set; }

        [Required]
        [MaxLength(2048)]
        public string DisplayDescription { get; set; }

        public int Bits { get; set; }

        public int? TagSetId { get; set; }
        public virtual TagSet TagSet { get; set; }
    }
}