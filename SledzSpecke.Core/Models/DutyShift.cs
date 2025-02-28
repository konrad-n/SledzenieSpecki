﻿using SledzSpecke.Core.Models.Enums;
using SQLite;
using System;

namespace SledzSpecke.Core.Models
{
    [Table("DutyShifts")]
    public class DutyShift
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public double DurationHours { get; set; }

        // New fields for SMK export format requirements
        [Ignore]
        public int DurationHoursInt => (int)Math.Floor(DurationHours);

        [Ignore]
        public int DurationMinutes => (int)Math.Round((DurationHours - DurationHoursInt) * 60);

        public DutyType Type { get; set; }

        public string Location { get; set; } // In SMK "Nazwa komórki organizacyjnej"

        public string SupervisorName { get; set; }

        public string Notes { get; set; }

        [Indexed]
        public int? InternshipId { get; set; }

        public int SpecializationId { get; set; }

        // Department name for SMK export
        public string DepartmentName { get; set; }
    }
}