using System;

namespace ClinicApp.Models
{
    public class BookAppointment
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
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public int? Age { get; set; }
        public string Gender { get; set; }
        public string Username { get; set; }
        public string Phonenumber { get; set; }
        public string Patientpwd { get; set; }

    }
}
