using HospitalWebApplication.Models.Doctors;
using HospitalWebApplication.Models.Identity;
using HospitalWebApplication.Models.Interfaces;
using HospitalWebApplication.Models.Patients;
using HospitalWebApplication.Models;

namespace HospitalWebApplication.Services
{
    public class DoctorService
    {
        private readonly IDoctorRepository _doctorRepository;

        public DoctorService(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public async Task<List<Appointment>> GetAppointmentsByDoctorAsync(int doctorId)
        {
            return await _doctorRepository.GetAppointmentsByDoctorAsync(doctorId);
        }

        public async Task<PROMIS10> GetPROMIS10ByAppointmentIDAsync(int appointmentID)
        {
            return await _doctorRepository.GetPROMIS10ByAppointmentIDAsync(appointmentID);
        }

        public async Task<Bill> GetBillByIdAsync(int billId)
        {
            return await _doctorRepository.GetBillByIdAsync(billId);
        }

        public async Task RejectAppointmentByIdAsync(int appointmentId)
        {
            var appointment = await _doctorRepository.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null) return;

            appointment.IsRejected = true;
            await _doctorRepository.RejectAppointmentAsync(appointment);
        }

        public async Task<List<BillItem>> GetBillItemsByBillIdAsync(int billId)
        {
            return await _doctorRepository.GetBillItemsByBillIdAsync(billId);
        }

        public async Task<Appointment> GetAppointmentByIdAsync(int appointmentId)
        {
            return await _doctorRepository.GetAppointmentByIdAsync(appointmentId);
        }

        public async Task<bool> CreateAsync(Doctor doctor)
        {
            return await _doctorRepository.CreateAsync(doctor);
        }

        public async Task<bool> UpdateBillAsync(Bill bill)
        {
            return await _doctorRepository.UpdateBillAsync(bill);
        }

        public async Task<bool> SendBillToPatientAsync(Bill bill)
        {
            bill.IsDoctorApproved = true;
            return await _doctorRepository.UpdateBillAsync(bill);
        }

        public async Task<bool> UpdateDoctorAsync(Doctor doctor)
        {
            return await _doctorRepository.UpdateDoctorAsync(doctor);
        }

        public async Task<List<Doctor>> GetAllDoctorsAsync()
        {
            return await _doctorRepository.GetAllDoctorsAsync();
        }

        public async Task<List<AppUser>> GetAllPatientsAsync()
        {
            return await _doctorRepository.GetAllPatientsAsync();
        }

        public async Task<bool> RemoveAsync(int doctorID)
        {
            return await _doctorRepository.RemoveAsync(doctorID);
        }

        public async Task<Doctor> GetDoctorByIDAsync(int doctorID)
        {
            return await _doctorRepository.GetDoctorByIDAsync(doctorID);
        }

        public async Task<bool> CreateAppointmentAsync(Appointment appointment)
        {
            return await _doctorRepository.CreateAppointmentAsync(appointment);
        }

        public async Task<Bill> CreateBillAsync(Bill bill)
        {
            var createdBill = await _doctorRepository.CreateBillAsync(bill);
            return await _doctorRepository.GetUpdatedBillByAppointmentIdAsync(bill.AppointmentId);
        }

        public async Task<Bill> GetBillByAppointmentIdAsync(int appointmentId)
        {
            return await _doctorRepository.GetUpdatedBillByAppointmentIdAsync(appointmentId);
        }

        public async Task<bool> CreateBillItemsAsync(BillItem bill)
        {
            return await _doctorRepository.CreateBillItemsAsync(bill);
        }

        public async Task<Doctor> GetDoctorByUserIDAsync(int userID)
        {
            return await _doctorRepository.GetDoctorByUserIDAsync(userID);
        }

        public async Task<bool> RemoveBillItemAsync(int billItemID, int billID)
        {
            var bill = await GetBillByIdAsync(billID);
            var billItem = await _doctorRepository.GetBillItemByIDAsync(billItemID);

            if (billItem == null || bill == null) return false;

            var removedItem = await _doctorRepository.RemoveBillItemAsync(billItemID);
            if (removedItem)
            {
                bill.Total -= billItem.Price;
                await _doctorRepository.UpdateBillAsync(bill);
            }

            return removedItem;
        }
    }
}
