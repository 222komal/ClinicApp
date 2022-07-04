using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ClinicApp.Models
{
    public partial class AllUsers
    {
        public string Username { get; set; }
        public int Id { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
