using ClinicApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
