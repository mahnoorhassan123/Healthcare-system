﻿using System;
using System.Collections.Generic;

namespace HealthCare.Data.Entity
{
    public partial class HealthCareUser
    {
        public HealthCareUser()
        {
            HealthcareDoctors = new HashSet<HealthcareDoctor>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool? Active { get; set; }
        public int? UserTypeId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? GenderId { get; set; }

        public virtual HealthCareGender Gender { get; set; }
        public virtual UserType UserType { get; set; }
        public virtual ICollection<HealthcareDoctor> HealthcareDoctors { get; set; }
    }
}