using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ClinicApp.Models
{
    public partial class Appointment
    {
        public int Apid { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public DateTime Datetime { get; set; }
        public string Duration { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool? Deleted { get; set; }
        public string Appointmentreason { get; set; }
        public TimeSpan? Duration2 { get; set; }
    }
}
