using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ClinicApp.Models
{
    public partial class Doctor
    {
        public int DoctorId { get; set; }
        public string DtFname { get; set; }
        public string DtLname { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhNumber { get; set; }
        public string Specilaity { get; set; }
        public int? Salary { get; set; }
        public string ClinicAvailbility { get; set; }
        public string Username { get; set; }
    }
}
