using ClinicApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Net.Http;

namespace ClinicApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonEndpointsController : ControllerBase
    {
        

        //List of doctors available to clinic
        [HttpGet("AvailableDoctors")]
        [Authorize(Roles = "Patient,Admin")]
        public IActionResult AvailableDoctors()
        {
            var clinicdata = new HospitalManagementSystemContext();
            var doctorslist1 = from doctor in clinicdata.Doctor where doctor.ClinicAvailbility.Trim().ToLower() == "yes"
                               select new
                               {
                                   Doctorname = doctor.DtFname + " " + doctor.DtLname,
                                   Speciality = doctor.Specilaity,
                                   ClinicAvaiability = doctor.ClinicAvailbility

                               };
            if (doctorslist1.Count() != 0)
            {
                return  Ok(doctorslist1);
            }
            return NotFound("No doctor is available at clinic");
            
           
        }      

        // Doctors and Admin can cancel appointment
        [HttpPut("CancelAppointment/{AppointmentId}")]
        [Authorize(Roles = "Doctor,Admin")]
        public IActionResult CancelAppointment(int AppointmentId)
        {
           
                var clinic_avalability = new HospitalManagementSystemContext();
                var appointment = clinic_avalability.Appointment.Where(o => o.Apid == AppointmentId).FirstOrDefault();
                if (appointment != null)
                {
                    appointment.Deleted = true;
                    clinic_avalability.Appointment.Update(appointment);
                    clinic_avalability.SaveChanges();
                    return Ok($"Appointment Cancel");
                }
                else if (appointment == null)
                {
                    return NotFound("Appointment not exist");
                }
                else
                {
                    return BadRequest();
                }
        }


        //Doctor and patient can View  appointment history
        [HttpGet("AppointmentHistory/{PatientId}")]
        [Authorize(Roles = "Doctor,Patient")]
        public IActionResult AppointmentHistory(int PatientId)
        {
            var clinicdata = new HospitalManagementSystemContext();
            var appointmenthistory = from doctor in clinicdata.Doctor
                                     join apointment in clinicdata.Appointment on doctor.DoctorId equals apointment.DoctorId
                                     join patient in clinicdata.Patient on apointment.PatientId equals patient.PatientId where apointment.PatientId == PatientId
                                     && apointment.Deleted == false
                                     select new
                                     {
                                         PatientId = patient.PatientId,
                                         Patient_firstname = patient.FirstName,
                                         Patient_lastname = patient.LastName,
                                         Doctor_Name = doctor.DtFname + " " + doctor.DtLname,
                                         Appointment_Date = apointment.Datetime,
                                         Disease = apointment.Appointmentreason
                                         
                                      };
            if (appointmenthistory != null) 
            {
                return Ok(appointmenthistory);
            }
           return NotFound("No Appointment History");
        }



        // Search All Doctors Schedule base on date
        [HttpGet("All_Doctors_Schedule/{schedul_edate}")]
        [Authorize(Roles = "Doctor,Admin")]
        public IActionResult All_Doctors_Schedule(DateTime schedul_edate)
        {      
            var clinicdata = new HospitalManagementSystemContext();
            var schedule = from doctor in clinicdata.Doctor
                           join apointment in clinicdata.Appointment on doctor.DoctorId equals apointment.DoctorId
                           join patient in clinicdata.Patient on apointment.PatientId equals patient.PatientId
                           where apointment.Datetime.Date == schedul_edate.Date && apointment.Deleted == false
                           select new
                           {
                               Appointment_Date = apointment.Datetime.Date,
                               Doctor_Name = doctor.DtFname + " " + doctor.DtLname,
                               DoctorSpeciality = doctor.Specilaity,
                               PatientId = patient.PatientId,
                               Patient_firstname = patient.FirstName,
                               Patient_lastname = patient.LastName,
                               StartTime = apointment.StartTime.ToShortTimeString(),
                               EndTime = apointment.EndTime.ToShortTimeString(),
                               Disease = apointment.Appointmentreason

                           };
            if (schedule.Count() != 0)
            {
                return Ok(schedule);
            }
            return NotFound("Schedule is empty");          
        }


        //Search Specifc doctor schedule on base of date
        [HttpGet("SearchDoctorSchedule")]
        [Authorize(Roles = "Doctor,Admin")]
        public IActionResult SearchDoctorSchedule([FromBody] ScheduleSearchmodel scheduleSearchmodel)
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
                               DoctorSpeciality = doctor.Specilaity,
                               PatientId = patient.PatientId,
                               Patient_firstname = patient.FirstName,
                               Patient_lastname = patient.LastName,
                               StartTime = apointment.StartTime.ToShortTimeString(),
                               EndTime = apointment.EndTime.ToShortTimeString(),
                               Disease = apointment.Appointmentreason
                           };
            if (schedule.Count() != 0)
            {
                return Ok(schedule);
            }
            return NotFound($"This doctor has no appoinment on {scheduleSearchmodel.scheduldate.Date}");
            
        }


        // Search doctor on base of Specialty
        [HttpGet("Speciality/{speciality}")]
        [Authorize(Roles = "Patient,Admin")]


        // Search doctors based on speciality
        public IActionResult Speciality(string speciality)
        {
            var clinicdata = new HospitalManagementSystemContext();
            var doctorslist1 = clinicdata.Doctor.Where(e => e.Specilaity == speciality).Select(x => new { x.DtFname, x.DtLname, x.ClinicAvailbility });

            if (doctorslist1.Count() != 0)
            {
                return Ok(doctorslist1);
            }
            return NotFound($"Doctor fot this {speciality} is not present");
        }

    }

}
