using HospitalWebApplication.Helpers.Enums;
using HospitalWebApplication.Models.Admins.ViewModels;
using HospitalWebApplication.Models.Identity;
using HospitalWebApplication.Models.Patients.ViewModels;
using HospitalWebApplication.Models.Patients;
using HospitalWebApplication.Models;
using HospitalWebApplication.Services;
using Microsoft.AspNetCore.Mvc;

namespace HospitalWebApplication.Controllers
{
    public class PatientController : Controller
    {
        private readonly PatientService _patientService;
        private readonly UserService _userService;
        private readonly DoctorService _doctorService;
        private readonly AppUser _currentUser;

        public PatientController(PatientService patientService, UserService userService, DoctorService doctorService)
        {
            _patientService = patientService;
            _userService = userService;
            _doctorService = doctorService;
            _currentUser = userService.GetCurrentUser();
        }

        public async Task<IActionResult> SelectInsuranceAndDiscountAsync(BillAndTransactionDetailsViewModelForm billDetailsAndForm)
        {
            var bill = billDetailsAndForm.BillData;
            var percentageOff = Convert.ToDecimal(InsuranceTypesEnum.InsuranceCoverage.ElementAt(bill.InsuranceId - 1).Value);
            var discountedTotal = InsuranceTypesEnum.CalculatePercentage(bill.Total, percentageOff);
            bill.OriginalTotal = bill.Total;
            bill.Total = discountedTotal;
            bill.InsuranceId = billDetailsAndForm.BillData.InsuranceId;

            await _patientService.UpdateBillAsync(bill);
            return RedirectToAction("BillPayment", "Patient", new { appointmentId = bill.AppointmentId });
        }

        public async Task<IActionResult> BillPaymentAsync(int appointmentId)
        {
            var bill = await _patientService.GetBillByAppointmentIdAsync(appointmentId);
            var billItems = await _patientService.GetBillItemsByBillIdAsync(bill.Id);

            var billDetails = new BillAndTransactionDetailsViewModelForm
            {
                BillData = bill,
                BillItems = billItems
            };

            return View(billDetails);
        }

        public async Task<IActionResult> CreateTransactionAsync(BillAndTransactionDetailsViewModelForm transactionFormInfo)
        {
            var transaction = transactionFormInfo.TransactionForm;
            var isCreated = await _patientService.CreateTransactionAsync(transaction, transactionFormInfo.AppointmentId);

            if (!isCreated)
            {
                return RedirectToAction("BillDetails", new { appointmentId = transactionFormInfo.AppointmentId });
            }

            return RedirectToAction("PatientBills");
        }

        public async Task<IActionResult> MyAppointmentsAsync()
        {
            var ivm = new IndexViewModel
            {
                ActiveAppointments = await _patientService.GetActiveAppointmentsByUserIdAsync(_currentUser.Id)
            };

            return View(ivm);
        }

        public async Task<IActionResult> DoctorIndexAsync()
        {
            var doctors = await _patientService.GetDoctorsDropdownAsync();
            return View(doctors);
        }

        [HttpGet]
        public async Task<IActionResult> BookAppointmentAsync(int appointmentId, PROMIS10 promis)
        {
            var appointment = await _patientService.GetAppointmentByIdAsync(appointmentId);
            appointment.UserID = _currentUser.Id;
            promis.AppointmentId = appointmentId;

            var appointmentForm = new CreateAppointmentViewModel
            {
                Appointment = appointment
            };

            return View(appointmentForm);
        }

        [HttpPost]
        public async Task<IActionResult> BookAppointmentAsync(Appointment appointment, List<int> Answers, List<int> QuestionIds, int AppointmentId)
        {
            var promis10 = _patientService.ConvertListToPROMIS10(Answers);
            var createdAppointment = await _patientService.BookAppointmentAsync(appointment, promis10);

            if (!createdAppointment)
            {
                return View(appointment);
            }

            return RedirectToAction("MyAppointments");
        }


        public async Task<IActionResult> PatientBillsAsync()
        {
            var patientBills = await _patientService.GetPatientBillsAsync(_currentUser.Id);
            return View(patientBills);
        }

        public async Task<IActionResult> AppointmentDetailsAsync(int appointmentID)
        {
            var appointment = await _patientService.GetAppointmentByIdAsync(appointmentID);
            var bill = await _patientService.GetBillByAppointmentIdAsync(appointmentID);
            var billItems = bill != null ? await _patientService.GetBillItemsByBillIdAsync(bill.Id) : new List<BillItem>();
            var promis10 = await _patientService.GetPROMIS10ByAppointmentIdAsync(appointmentID);

            var appointmentDetailsViewModel = new AppointmentDetailsViewModel
            {
                Appointment = appointment,
                Bill = bill,
                PROMIS10 = promis10,
                BillItemsAdded = billItems
            };

            return View(appointmentDetailsViewModel);
        }

        public async Task<IActionResult> DoctorAndAppointmentsDetailsAsync(int doctorID)
        {
            var doctorDetailsViewModel = new DoctorDetailsViewModel
            {
                Doctor = await _doctorService.GetDoctorByIDAsync(doctorID),
                DoctorAppointments = (await _patientService.GetAppointmentsByDoctorIdAsync(doctorID))
                    .Where(x => !x.IsBooked && !x.IsRejected && x.AppointmentDate.Date >= DateTime.Today)
                    .ToList(),

            };

            doctorDetailsViewModel.OrganizeAppointmentsByMonths();
            return View(doctorDetailsViewModel);
        }
    }
}
