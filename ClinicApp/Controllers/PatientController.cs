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
            if (newuser != null)
            {
                var clinicdata = new HospitalManagementSystemContext();
                var currentUser = clinicdata.AllUsers.FirstOrDefault(o => o.Username.ToLower() == newuser.Username.ToLower());
                if (currentUser != null && currentUser.Username.ToLower() == newuser.Username.ToString().ToLower())
                {
                    return BadRequest("User Name must be unique");
                }
                AllUsers Users = new AllUsers();
                Users.Username = newuser.Username;
                Users.Password = newuser.Password;
                Users.Role =  "Patient";
                clinicdata.AllUsers.Add(Users);
                clinicdata.SaveChanges();
                return Ok("Registered");
            }
            return BadRequest("Error");

        }
       // [Authorize(Roles = "Patient")]
        [HttpPost("BookAppoinment")]
        public IActionResult BookAppoinment([FromBody] BookAppointment appointment)
        {
            if (appointment != null)
            {
                var clinicdata = new HospitalManagementSystemContext();
                //var results = from apointment in clinicdata.Appointment
                //              group apointment by apointment.DoctorId into g
                //              select new
                //              {
                //                  ProductName = g.First().DoctorId,
                //                  Price = g.Sum(pc => TimeSpan.Parse(pc.Duration).Minutes).ToString(),
                //                  Quantity = g.Count().ToString(),
                //              };

                
                TimeSpan ts = appointment.EndTime - appointment.StartTime;

                if (ts.TotalMinutes < 15)
                {
                    return BadRequest("Minimum appointment duration must be 15 minutes");
                }
                 else if(ts.TotalMinutes > 120)
                {
                    return BadRequest("Maximum appointment duration must be 2 hrs");
                }
                else if ( clinicdata.Appointment.Where(e => e.DoctorId == appointment.DoctorId && e.Datetime.Date == appointment.Datetime.Date).Count() > 12)
                {
                    return BadRequest("appointments  limit reached to 12 ,SorryNo Slot free");
                }
                else
                {
                    Appointment appointtment_booked = new Appointment();         
                    appointtment_booked.DoctorId = appointment.DoctorId;
                    appointtment_booked.Appointmentreason = appointment.Appointmentreason;
                    appointtment_booked.Datetime = appointment.Datetime;
                    appointtment_booked.StartTime = appointment.StartTime;
                    appointtment_booked.EndTime = appointment.EndTime;
                    appointtment_booked.Duration = ts.ToString();
                    appointtment_booked.Deleted = false;
                    var currentUser = clinicdata.Patient.FirstOrDefault(o => o.Username.ToLower() == appointment.Username.ToLower());
                    //
                    if (currentUser != null && currentUser.Username.ToLower() == currentUser.Username.ToString().ToLower())
                    {
                        //patient  already exist just  update patient info and creat new appointment
                        currentUser.FirstName = appointment.FirstName;
                        currentUser.LastName = appointment.LastName;
                        currentUser.Address = appointment.Address;
                        currentUser.Age = appointment.Age;
                        currentUser.Gender = appointment.Gender;
                        currentUser.Phonenumber = appointment.Phonenumber;
                        currentUser.Username = appointment.Username;
                        appointtment_booked.PatientId = currentUser.PatientId;
                        clinicdata.Patient.Update(currentUser);
                        clinicdata.Appointment.Add(appointtment_booked);
                        clinicdata.SaveChanges();
                    }
                    else
                    {
                        // Create New Pateint 
                        Patient patient = new Patient();
                        patient.FirstName = appointment.FirstName;
                        patient.LastName = appointment.LastName;
                        patient.Address = appointment.Address;
                        patient.Age = appointment.Age;
                        patient.Gender = appointment.Gender;
                        patient.Phonenumber = appointment.Phonenumber;
                        patient.Username = appointment.Username;
                        int PatientId = Convert.ToInt32(clinicdata.Patient.Max(e => e.PatientId)) +1;
                        appointtment_booked.PatientId = PatientId;
                        clinicdata.Patient.Add(patient);
                        clinicdata.Appointment.Add(appointtment_booked);
                        clinicdata.SaveChanges();
                    }
       
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
