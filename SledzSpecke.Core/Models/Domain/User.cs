﻿using SQLite;
using System;
using System.Collections.Generic;

namespace SledzSpecke.Core.Models.Domain
{
    [Table("User")]
    public class User : BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public override int Id { get; set; }

        [Column("Email")]
        public string Email { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("PWZ")]
        public string PWZ { get; set; } // Numer prawa wykonywania zawodu

        [Column("CurrentSpecializationId")]
        public int? CurrentSpecializationId { get; set; }

        [Column("SpecializationStartDate")]
        public DateTime SpecializationStartDate { get; set; }

        [Column("ExpectedEndDate")]
        public DateTime ExpectedEndDate { get; set; }

        [Column("SupervisorId")]
        public int? SupervisorId { get; set; }

        // Właściwości nawigacyjne
        [Ignore]
        public Specialization CurrentSpecialization { get; set; }

        [Ignore]
        public User Supervisor { get; set; }

        [Ignore]
        public ICollection<User> Supervisees { get; set; }
    }
}
