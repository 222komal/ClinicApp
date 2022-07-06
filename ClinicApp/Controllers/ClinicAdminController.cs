using ClinicApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace ClinicApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicAdminController : ControllerBase
    {

        [Authorize(Roles = "Admin")]
        [HttpPost("AccountRegister")]
        public IActionResult AccountRegister([FromBody] UserModel newuser)
        {

            if (ModelState.IsValid)
            {
                var clinicdata = new HospitalManagementSystemContext();
                //Check unique user name should be unique
                var currentUser = clinicdata.AllUsers.FirstOrDefault(o => o.Username.ToLower() == newuser.Username.ToLower());
                if( currentUser != null && currentUser.Username.ToLower() == newuser.Username.ToString().ToLower())
                {
                    return BadRequest("User Name must be unique");
                }
                AllUsers allUsers = new AllUsers();
                allUsers.Username = newuser.Username;
                allUsers.Password = newuser.Password;
                allUsers.Role = newuser.Role;

                if (newuser.Role.Trim().ToLower() == "doctor")
                {
                    Doctor doctor = new Doctor();
                    doctor.Salary = newuser.Salary;
                    doctor.Address = newuser.Address;
                    doctor.DtFname = newuser.Fname;
                    doctor.DtLname = newuser.Lname;
                    doctor.Email = newuser.EmailAddress;
                    doctor.PhNumber = newuser.PhNumber;
                    doctor.ClinicAvailbility = "No"; //By default no 
                    doctor.Specilaity = newuser.Specilaity;
                    clinicdata.Doctor.Add(doctor);
                    //user role
                    allUsers.Role = "Doctor";
                    clinicdata.AllUsers.Add(allUsers);
                    clinicdata.SaveChanges();
                    return Ok("Account Registered");
                }
                else if (newuser.Role.Trim().ToLower() == "admin")
                {
                    ClinicAdmin admin = new ClinicAdmin();
                   
                    admin.Address = newuser.Address;
                    admin.AdFname = newuser.Fname;
                    admin.AdLname = newuser.Lname;
                    admin.Email = newuser.EmailAddress;
                    admin.PhNumber = newuser.PhNumber;
                    clinicdata.ClinicAdmin.Add(admin);
                    // set user role
                    allUsers.Role = "Admin";
                    clinicdata.AllUsers.Add(allUsers);
                    clinicdata.SaveChanges();
                    return Ok("Account Registered");
                }
                else
                {
                    return BadRequest("Account not registered");
                }
            }
            else
            {
                return BadRequest();
            }
       
        }



        // doctors appointment and their count shown on specifc date
        [Authorize(Roles = "Admin")]
        [HttpGet("MaxAppointment/{dateTime}")]
        public IActionResult MaxAppointment(DateTime dateTime)
        {
            var clinicdata = new HospitalManagementSystemContext();
            var res = from appt in clinicdata.Appointment
                      join doctor in clinicdata.Doctor
                      on appt.DoctorId equals doctor.DoctorId
                      where appt.Datetime.Date == dateTime.Date && (appt.Deleted == false || appt.Deleted == null)
                      select new
                      {
                          appt,
                          doctor,
                          DurationInHours = (appt.EndTime - appt.StartTime).TotalHours
                      };
            var lst = res.ToList();
            var result = lst.GroupBy(x => new
            {
                DoctorId = x.doctor.DoctorId,
                DtFname = x.doctor.DtFname,
                DtLname = x.doctor.DtLname,
                ApptDate = x.appt.Datetime.Date
            })
                .Select(r => new {
                    DoctorId = r.Key.DoctorId,
                    ApptDate = r.Key.ApptDate,
                    Doctorname = r.Key.DtFname + " " + r.Key.DtLname,
                    AppointmentCount = r.Count(),
                    durationin_hours = r.Sum(y => y.DurationInHours)
                }).OrderByDescending(r => r.durationin_hours).ToList();

           
            if (result.Count() != 0)
            {
                return Ok(result);

            }
            return NotFound("Empty");

        }

        // doctors appointment exceeding 6 hours on specifc date
        [Authorize(Roles = "Admin")]
        [HttpGet("AppointmentExceeding_6Hrs/{dateTime}")]
        public IActionResult AppointmentExceeding_6Hrs(DateTime dateTime)
        {
            var clinicdata = new HospitalManagementSystemContext();
            var res = from appt in clinicdata.Appointment
                      join doctor in clinicdata.Doctor
                      on appt.DoctorId equals doctor.DoctorId
                      where appt.Datetime.Date == dateTime.Date && (appt.Deleted == false || appt.Deleted == null)
                      select new
                      {
                          appt,
                          doctor,
                          DurationInHours = (appt.EndTime - appt.StartTime).TotalHours
                      };
         
            var lst = res.ToList();
            var result = lst.GroupBy(x => new
            {
                DoctorId = x.doctor.DoctorId,
                DtFname = x.doctor.DtFname,
                DtLname = x.doctor.DtLname,
                ApptDate = x.appt.Datetime.Date
            })
                .Select(r => new {
                    DoctorId = r.Key.DoctorId,
                    ApptDate = r.Key.ApptDate,
                    Doctorname = r.Key.DtFname + " " + r.Key.DtLname,
                    countappointments = r.Count(),
                    DurationInHours = r.Sum(y => y.DurationInHours)
                }).OrderByDescending(r => r.DurationInHours).ToList();

            var finalyresult = result.Where(r => r.DurationInHours > Convert.ToDouble(6));
            if (finalyresult.Count() != 0)
            {
                return Ok(finalyresult);

            }
            else
            {
                return NotFound();
            }

        }


        //Search Doctor Information
        [HttpGet("AllDoctors")]
        [Authorize(Roles = "Admin")]
        public IActionResult SearchDoctor_info(int id)
        {
            var clinicdata = new HospitalManagementSystemContext();
            var doctorslist1 = from doctor in clinicdata.Doctor where doctor.DoctorId == id
                               select new
                               {
                                   Doctorname = doctor.DtFname + " " + doctor.DtLname,
                                   Speciality = doctor.Specilaity,
                                   ClinicAvaiability = doctor.ClinicAvailbility,
                                   Email = doctor.Email,
                                   Phone = doctor.PhNumber,
                                   Username = doctor.Username,
                                   Salary = doctor.Salary

                               };

            if (doctorslist1.Count() != 0)
            {
                return Ok(doctorslist1);
            }
            return NotFound();
        }



        //List of All Doctor at clinic both available and not available
        [HttpGet("AllDoctors")]
        [Authorize(Roles = "Admin")]
        public IActionResult AllDoctors()
        {
            var clinicdata = new HospitalManagementSystemContext();
            var doctorslist1 = from doctor in clinicdata.Doctor
                               select new
                               {
                                   Doctorname = doctor.DtFname + " " + doctor.DtLname,
                                   Speciality = doctor.Specilaity,
                                   ClinicAvaiability = doctor.ClinicAvailbility,
                                   Email = doctor.Email,
                                   Phone = doctor.PhNumber

                               };

            if (doctorslist1.Count() != 0)
            {
                return Ok(doctorslist1);
            }
            return NotFound();
        }


    }
}
