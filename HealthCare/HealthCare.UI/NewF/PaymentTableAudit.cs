﻿using System;
using System.Collections.Generic;

namespace HealthCare.UI.NewF
{
    public partial class PaymentTableAudit
    {
        public int AuditId { get; set; }
        public int? PaymentId { get; set; }
        public int? PatientUserId { get; set; }
        public int? ProviderId { get; set; }
        public decimal? Amount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime? Timestamp { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool? Active { get; set; }
        public int? UserId { get; set; }
        public string AuditAction { get; set; }
        public DateTime? AuditTimestamp { get; set; }
        public int? OldPatientUserId { get; set; }
        public int? OldProviderId { get; set; }
        public decimal? OldAmount { get; set; }
        public string OldPaymentMethod { get; set; }
        public DateTime? OldTimestamp { get; set; }
        public DateTime? OldCreatedAt { get; set; }
        public DateTime? OldUpdatedAt { get; set; }
        public bool? OldActive { get; set; }

        public virtual UserTable OldPatientUser { get; set; }
        public virtual HealthcareProviderTable OldProvider { get; set; }
        public virtual UserTable User { get; set; }
    }
}