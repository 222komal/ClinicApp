using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ClinicApp.Models
{
    public partial class Patient
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public int? Age { get; set; }
        public string Gender { get; set; }
        public int PatientId { get; set; }
        public string Username { get; set; }
        public string Phonenumber { get; set; }
    }
}
