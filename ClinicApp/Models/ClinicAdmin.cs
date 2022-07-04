using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ClinicApp.Models
{
    public partial class ClinicAdmin
    {
        public int AdminId { get; set; }
        public string AdFname { get; set; }
        public string AdLname { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhNumber { get; set; }
    }
}
