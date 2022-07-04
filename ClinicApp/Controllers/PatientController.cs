using ClinicApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClinicApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {


        [Authorize(Roles = "Patient")]
        [HttpPost("RegisterPatient")]
        public IActionResult RegisterPatient([FromBody] UserModel newuser)
        {

            return Ok();

        }
      //  [Authorize(Roles = "Patient")]
        [HttpPost("BookAppoinment")]
        public IActionResult BookAppoinment([FromBody] BookAppointment appointment)
        {

            if (appointment != null)
            {
               
                TimeSpan ts = appointment.EndTime - appointment.StartTime;

                if (ts.TotalMinutes < 15)
                {
                    return BadRequest("Minimum appointment duration must be 15 minutes");
                }
                 else if(ts.TotalMinutes > 120)
                {
                    return BadRequest("Maximum appointment duration must be 2 hrs");
                }
                else
                {
                    var clinicdata = new HospitalManagementSystemContext();
                    Patient patient = new Patient();
                    patient.FirstName = appointment.FirstName;
                    patient.LastName = appointment.LastName;
                    patient.Address = appointment.Address;
                    patient.PatientId = Convert.ToInt32(clinicdata.Patient.Max(e => e.PatientId));

                    patient.Age = appointment.Age;
                    patient.Gender = appointment.Gender;
                    patient.Phonenumber = appointment.Phonenumber;
                    patient.Username = appointment.Username;
                    //User Object Creation
                    AllUsers User = new AllUsers();
                    User.Username = appointment.Username;
                    User.Password = appointment.Patientpwd;
                    User.Role = "Patient";
                    clinicdata.AllUsers.Add(User);
                    clinicdata.Patient.Add(patient);
                    //Appointment object
                    var count = clinicdata.Appointment.Where(e => e.DoctorId==1 && e.PatientId==1).Count();
                    var ab = clinicdata.Appointment.Where( e => e.DoctorId==1 && e.PatientId==1);
                 //   var ab1 = clinicdata.Appointment.Where(e => e.PatientId == 1).Sum(e => TimeSpan (e.Duration2));
                    Appointment appointtment_booked = new Appointment();
                    appointtment_booked.PatientId= patient.PatientId;
                    appointtment_booked.DoctorId = appointment.DoctorId;
                    appointment.Appointmentreason = appointment.Appointmentreason;
                    appointment.Datetime = appointment.Datetime;
                    appointtment_booked.StartTime = appointment.StartTime;
                    appointtment_booked.EndTime = appointment.EndTime;
                    appointment.Duration = ts.ToString();                  
                    appointtment_booked.Deleted = false;
                    clinicdata.Appointment.Add(appointtment_booked);
                    return Ok("Appointment Booked");
                }
                   
            }
            else
            {
                return BadRequest("Error Occured duriung booking appointment");
            }
               
        }
        //Patient search Appointment history
        [HttpGet("Patient_Search_Appointment/{PatientId}")]
        [Authorize(Roles = "Patient")]
        public IActionResult Patient_Search_Appointment(int PatientId)
        {
            var clinicdata = new HospitalManagementSystemContext();
            var Patient_ppointments = from doctor in clinicdata.Doctor join apointment in clinicdata.Appointment on doctor.DoctorId equals apointment.DoctorId
                                      join patient in clinicdata.Patient on apointment.PatientId equals patient.PatientId
                                      where apointment.PatientId == PatientId
                                     select new
                                     {
                                         DoctorId = doctor.DoctorId,
                                         Doctor_Name = doctor.DtFname + " " + doctor.DtLname,
                                         Appointment_Date = apointment.Datetime,
                                         ReasonOf_Appointment = apointment.Appointmentreason,
                                         StartTime = apointment.StartTime,
                                         EndTime = apointment.EndTime,

                                     };
            if (Patient_ppointments.Count() != 0)
            {
                return Ok(Patient_ppointments);
            }
            return NotFound("No Appointment History");
        }

        // Patient View  any Doctor's slot 
        [HttpGet("Any_Doctor_Slots")]
        [Authorize(Roles = "Patient")]
        public IActionResult Any_Doctor_Slots([FromBody] ScheduleSearchmodel scheduleSearchmodel)
        {
            var clinicdata = new HospitalManagementSystemContext();
            var schedule = from doctor in clinicdata.Doctor
                           join apointment in clinicdata.Appointment on doctor.DoctorId equals apointment.DoctorId
                           join patient in clinicdata.Patient on apointment.PatientId equals patient.PatientId
                           where apointment.Datetime.Date == scheduleSearchmodel.scheduldate.Date && apointment.DoctorId == scheduleSearchmodel.Id && apointment.Deleted == false
                           select new
                           {
                               Appointment_Date = apointment.Datetime.Date,
                               Doctor_Name = doctor.DtFname + " " + doctor.DtLname,
                               Speciality = doctor.Specilaity,
                               StartTime = apointment.StartTime.ToShortTimeString(),
                               EndTime = apointment.EndTime.ToShortTimeString(),
                               
                           };
            if (schedule.Count() != 0)
            {
                return Ok(schedule);
            }
            return NotFound($"This doctor has no appoinment on {scheduleSearchmodel.scheduldate.Date}");
        }
   

        // Search All Doctors Schedule base on date
        [HttpGet("Slots/{slot_date}")]
        [Authorize(Roles = "Patient")]
        public IActionResult Slots(DateTime slot_date)
        {
            var clinicdata = new HospitalManagementSystemContext();
            var schedule = from doctor in clinicdata.Doctor
                           join apointment in clinicdata.Appointment on doctor.DoctorId equals apointment.DoctorId
                           where apointment.Datetime.Date == slot_date.Date && apointment.Deleted == false
                           select new
                           {
                               Appointment_Date = apointment.Datetime.Date,
                               Doctor_Name = doctor.DtFname + " " + doctor.DtLname,
                               Speciality = doctor.Specilaity,
                               StartTime = apointment.StartTime.ToShortTimeString(),
                               EndTime = apointment.EndTime.ToShortTimeString(),
                               Disease = apointment.Appointmentreason,
                               
                           };
            if (schedule.Count() != 0)
            {
                return Ok(schedule);
            }
            return NotFound("Schedule is empty");
        }




        //Patient Search All  Doctors on clinic
        [HttpGet("PatientSearchDoctors")]
        [Authorize(Roles = "Patient")]
        public IActionResult PatientSearchDoctors()
        {
            var clinicdata = new HospitalManagementSystemContext();
          
            var doctorslist1 = from doctor in clinicdata.Doctor
                               select new
                               {
                                   Doctorname = doctor.DtFname + " " + doctor.DtLname,
                                   Speciality = doctor.Specilaity,
                                   ClinicAvaiability = doctor.ClinicAvailbility,
                            
                               };

            if (doctorslist1.Count() != 0)
            {
                return Ok(doctorslist1);
            }
            return NotFound();
        }

    }
}
