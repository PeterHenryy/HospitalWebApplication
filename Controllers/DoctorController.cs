using HospitalWebApplication.Models.Doctors.ViewModels;
using HospitalWebApplication.Models.Doctors;
using HospitalWebApplication.Models.Identity;
using HospitalWebApplication.Models;
using HospitalWebApplication.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWebApplication.Controllers
{
    public class DoctorController : Controller
    {
        private readonly DoctorService _doctorService;
        private readonly UserService _userService;
        private readonly AppUser _currentUser;
        private readonly UserManager<AppUser> _userManager;
        // private readonly IBlobService _blobService;

        public DoctorController(DoctorService doctorService, UserService userService, UserManager<AppUser> userManager)
        {
            _doctorService = doctorService;
            _userService = userService;
            _currentUser = userService.GetCurrentUser();
            _userManager = userManager;
            // _blobService = blobService;
        }

        [HttpGet]
        public IActionResult CreateAppointment()
        {
            List<string> appointmentTimes = new List<string>();

            for (int hour = 1; hour <= 12; hour++)
            {
                appointmentTimes.Add($"{hour}:00 AM");
            }

            for (int hour = 1; hour <= 12; hour++)
            {
                appointmentTimes.Add($"{hour}:00 PM");
            }

            ViewBag.AppointmentTimes = appointmentTimes;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment(Appointment appointment)
        {
            Doctor doctor = await _doctorService.GetDoctorByUserIDAsync(_currentUser.Id);
            appointment.DoctorID = doctor.ID;
            appointment.IsBooked = false;
            bool createdAppointment = await _doctorService.CreateAppointmentAsync(appointment);
            if (createdAppointment)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public async Task<IActionResult> EditDoctorProfile()
        {
            DoctorUpdateViewModelForm doctorViewModel = new DoctorUpdateViewModelForm();
            var doctor = await _doctorService.GetDoctorByUserIDAsync(_currentUser.Id);
            doctorViewModel.Doctor = doctor;
            doctorViewModel.AppUser = _currentUser;
            return View(doctorViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditDoctorProfile(DoctorUpdateViewModelForm doctorViewModelForm)
        {
            //
            var doctorObject = await _doctorService.GetDoctorByIDAsync(doctorViewModelForm.Doctor.ID);
            var updatedUser = doctorViewModelForm.AppUser;
            var files = HttpContext.Request.Form.Files;
            if (files.Count > 0)
            {
                updatedUser.ProfilePicture = files[0].FileName;
                _userService.HandleUserProfilePicture(files);
                //    bool uploadedBlob = await _blobService.UploadBlob(updatedUser.ProfilePicture, files[0], new Blob());
                //    doctorObject.ProfilePictureURI = _blobService.GetBlob(updatedUser.ProfilePicture);
            }
            updatedUser.SecurityStamp = Guid.NewGuid().ToString();
            AppUser mappedUser = await _userService.MapUserUpdates(updatedUser, _currentUser);
            var user = await _userManager.UpdateAsync(mappedUser);

            // Update Doctor
            if (doctorViewModelForm.Doctor.Bio != null)
            {
                // checking if they really submitted any new bios, otherwise we skip this 
                if (doctorViewModelForm.Doctor.Bio.Length > 1)
                {
                    doctorObject.Bio = doctorViewModelForm.Doctor.Bio;
                }
            }

            bool updatedDoctor = await _doctorService.UpdateDoctorAsync(doctorObject);

            return RedirectToAction("MyAppointments", "Doctor");
        }

        public async Task<IActionResult> MyAppointments()
        {
            var doctor = await _doctorService.GetDoctorByUserIDAsync(_currentUser.Id);
            var doctorID = doctor.ID;
            var appointments = await _doctorService.GetAppointmentsByDoctorAsync(doctorID);
            return View(appointments);
        }
        public async Task<IActionResult> BillingForAppointment(int appointmentId)
        {
            // Grabbing appointment Object which we are billing
            var appointment = await _doctorService.GetAppointmentByIdAsync(appointmentId);

            // Bill that will be passed in 
            var billObject = new Bill(); // Temp holder

            // Checking if bill exists by appointment Id
            var billExist = await _doctorService.GetBillByAppointmentIdAsync(appointmentId);

            // If Bill doesns't exist, create a new one & pass it in
            if (billExist == null)
            {
                // Creating NEW Bill object for first time
                Bill bill = new Bill();
                bill.Appointment = appointment;
                bill.AppointmentId = appointment.ID;
                bill.Total = appointment.InitialTotal;
                bill.OriginalTotal = appointment.InitialTotal;
                // Creating Bill to have id as reference for BillItems
                billObject = await _doctorService.CreateBillAsync(bill);
            }
            else
            {
                // If already exist in the system, pass it in
                billObject = billExist;
            }
            // We are grabbing all related items/medication to this bill & updating the price on Bill
            var billItems = await _doctorService.GetBillItemsByBillIdAsync(billObject.Id);

            // Creating ViewModel & passing info in
            BillViewModel billViewModel = new BillViewModel();
            billObject.Appointment.User = await _userManager.FindByIdAsync(appointment.UserID.ToString());
            billViewModel.BillInfo = billObject;
            billViewModel.BillItemsAdded = billItems;
            billViewModel.BillInfo.Total = billObject.Total;
            billViewModel.Promis = await _doctorService.GetPROMIS10ByAppointmentIDAsync(appointmentId);
            return View(billViewModel);
        }
        public async Task<IActionResult> CreateBillItems(BillViewModel billViewModel)
        {
            // Creating Bill Item
            BillItem newbillItem = billViewModel.BillItemForm;

            bool createdBill = await _doctorService.CreateBillItemsAsync(newbillItem);

            // Getting Bill
            var bill = await _doctorService.GetBillByIdAsync(newbillItem.BillId);
            // Updating Total
            bill.Total = bill.Total + newbillItem.Price;
            // Updating Record in Database
            await _doctorService.UpdateBillAsync(bill);
            // Redirect
            return RedirectToAction("BillingForAppointment", new { appointmentId = billViewModel.BillInfo.AppointmentId });
        }
        public async Task<IActionResult> SendBillToPatient(BillViewModel bill)
        {
            // This column indicates if bill is ready to be seen by Patient
            var isCreated = await _doctorService.SendBillToPatientAsync(bill.BillForm);
            if (!isCreated)
            {
                return RedirectToAction("BillingForAppointment");
            }
            return RedirectToAction("MyAppointments");

        }
        public async Task<IActionResult> RejectAppointment(int appointmentId)
        {
            await _doctorService.RejectAppointmentByIdAsync(appointmentId);
            return RedirectToAction("DoctorAppointments", "Admin");
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> RemoveBillItem(int billItemID, int appointmentID, int billID)
        {
            bool removedItem = await _doctorService.RemoveBillItemAsync(billItemID, billID);
            return RedirectToAction("BillingForAppointment", "Doctor", new { appointmentID = appointmentID });
        }

    }
}
