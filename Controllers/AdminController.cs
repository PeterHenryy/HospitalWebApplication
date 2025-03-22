using HospitalWebApplication.Helpers.Enums;
using HospitalWebApplication.Models.Admins.ViewModels;
using HospitalWebApplication.Models.Doctors.ViewModels;
using HospitalWebApplication.Models.Doctors;
using HospitalWebApplication.Models.Identity;
using HospitalWebApplication.Models.Patients;
using HospitalWebApplication.Models;
using HospitalWebApplication.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWebApplication.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly DoctorService _doctorService;
        private readonly PatientService _patientService;
        private readonly AdminService _adminService;
        //private readonly IBlobService _blobService;
        private readonly UserService _userService;
        private readonly AppUser _currentUser;

        public AdminController(UserManager<AppUser> userManager, UserService userService, DoctorService doctorService, PatientService patientService, AdminService adminService)
        {
            _userManager = userManager;
            _userService = userService;
            _currentUser = userService.GetCurrentUser();
            _doctorService = doctorService;
            _patientService = patientService;
            _adminService = adminService;
            //_blobService = blobService;
        }

        [HttpGet]
        public IActionResult RegisterDoctor()
        {
            var registerDoctorForm = new RegisterDoctorAndUserViewModelForm();
            return View(registerDoctorForm);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterDoctor(RegisterDoctorAndUserViewModelForm doctor)
        {
                
                var existingUserName = await _userManager.FindByNameAsync(doctor.UserDataForm.UserName);
                if (existingUserName != null)
                {   
                    ModelState.AddModelError("userName", "A doctor with this username already exists or was previously removed");
                    var registerDoctorForm = new RegisterDoctorAndUserViewModelForm();
                    return View(registerDoctorForm);
                }
                var existingUserEmail = await _userManager.FindByEmailAsync(doctor.UserDataForm.Email);
                if (existingUserEmail != null)
                {
                    // If email is already taken, add a model error
                    ModelState.AddModelError("Email", "A doctor with this email already exists or was previously removed ");
                    var registerDoctorForm = new RegisterDoctorAndUserViewModelForm();
                    return View(registerDoctorForm);
            }
                // Creating AppUser and registering to database with "Doctor" role
                AppUser appUser = doctor.UserDataForm;
                appUser.ProfilePicture = "user-doctor-solid.svg";

                // Creating role 
                var role = UserRolesEnum.Doctor.ToString();

                // Creating user
                var userRegister = await _userManager.CreateAsync(appUser);

                // Assigning doctor role to user
                var assignRole = await _userManager.AddToRoleAsync(appUser, role);

                // Creating doctor instance and registering it to database
                Doctor newDoctor = doctor.DoctorDataForm;
                newDoctor.User = appUser;
                newDoctor.Bio = "Specialist at Ineza Physiotherapy Clinic";
                newDoctor.ProfilePictureURI = "/profilepics/user-doctor-solid.svg";

                bool addedDoctor = await _doctorService.CreateAsync(newDoctor);
            
                return RedirectToAction("DoctorsIndex", "Admin");

        }

        public async Task<IActionResult> DoctorsIndex()
        {
            List<Doctor> doctors = await _doctorService.GetAllDoctorsAsync();
            List<decimal> revenues = new List<decimal>();
            foreach (var doctor in doctors)
            {
                decimal doctorRevenue = await _adminService.CalculateDoctorRevenueAsync(doctor.ID);
                revenues.Add(doctorRevenue);
            }
            ViewBag.DoctorRevenues = revenues;
            ViewBag.Revenue = await _adminService.CalculateHospitalRevenueAsync();
            return View(doctors);
        }
        public async Task<IActionResult> PatientsIndex()
        {
            List<AppUser> patients = await _doctorService.GetAllPatientsAsync();
            return View(patients);
        }

        public async Task<IActionResult> Remove(int doctorID, int userID)
        {
            bool removedDoctor = await _doctorService.RemoveAsync(doctorID);
            return RedirectToAction("DoctorsIndex", "Admin");
        }

        public async Task<IActionResult> DisplayPatients()
        {
            IList<AppUser> patients = await _userManager.GetUsersInRoleAsync(UserRolesEnum.Patient.ToString());
            return View(patients);
        }

        public async Task<IActionResult> DoctorAppointments(int doctorID)
        {
            List<Appointment> doctorAppointments = await _doctorService.GetAppointmentsByDoctorAsync(doctorID);
            return View(doctorAppointments);
        }

        public async Task<IActionResult> AppointmentDetails(int appointmentID)
        {
            List<BillItem> billItems = new List<BillItem>();
            Appointment appointment = await _doctorService.GetAppointmentByIdAsync(appointmentID);
            appointment.User = await _userManager.FindByIdAsync(appointment.UserID.ToString());
            Bill bill = await _doctorService.GetBillByAppointmentIdAsync(appointmentID);
            if (bill != null)
            {
                billItems = await _doctorService.GetBillItemsByBillIdAsync(bill.Id);
                bill.Appointment.User = await _userManager.FindByIdAsync(appointment.UserID.ToString());
            }
            PROMIS10 promis10 = await _patientService.GetPROMIS10ByAppointmentIdAsync(appointmentID);
            List<Doctor> doctors = await _doctorService.GetAllDoctorsAsync();
            var appointmentDetailsViewModel = new AppointmentDetailsViewModel();
            appointmentDetailsViewModel.Appointment = appointment;
            appointmentDetailsViewModel.Bill = bill;
            appointmentDetailsViewModel.PROMIS10 = promis10;
            appointmentDetailsViewModel.BillItemsAdded = billItems;
            appointmentDetailsViewModel.Doctors = doctors;
            return View(appointmentDetailsViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> ReassignDoctorToAppointment(AppointmentDetailsViewModel appointmentDetailsViewModel)
        {
            Appointment appointment = await _doctorService.GetAppointmentByIdAsync(appointmentDetailsViewModel.AppointmentID);
            appointment.DoctorID = appointmentDetailsViewModel.DoctorID;
            bool updatedAppointment = await _patientService.UpdateAppointmentAsync(appointment);
            return RedirectToAction("AppointmentDetails", "Admin", new { appointmentID = appointmentDetailsViewModel.AppointmentID });
        }

        [HttpGet]
        public IActionResult CreateAppointment(int doctorId)
        {
            List<string> appointmentTimes = new List<string>();

            for (int hour = 1; hour <= 11; hour++)
            {
                appointmentTimes.Add($"{hour}:00 AM");
            }
            appointmentTimes.Add("12:00 PM");
            for (int hour = 1; hour <= 11; hour++)
            {
                appointmentTimes.Add($"{hour}:00 PM");
            }
            appointmentTimes.Add("12:00 AM");

            ViewBag.AppointmentTimes = appointmentTimes;
            Appointment appointment = new Appointment()
            {
                DoctorID = doctorId
            };
            return View(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment(Appointment appointment)
        {
            appointment.IsBooked = false;
            bool createdAppointment = await _doctorService.CreateAppointmentAsync(appointment);
            if (createdAppointment)
            {
                return RedirectToAction("DoctorsIndex", "Admin");
            }
            return View();
        }

        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            var appointment = await _patientService.GetAppointmentByIdAsync(appointmentId);
            appointment.IsRejected = true;
            var isUpdated = await _patientService.UpdateAppointmentAsync(appointment);
            return RedirectToAction("DoctorAppointments", "Admin", new { doctorID = appointment.DoctorID });
        }
    }
}
