using HospitalWebApplication.Models.Doctors;
using HospitalWebApplication.Models.Patients;

namespace HospitalWebApplication.Models.Interfaces
{
    public interface IPatientRepository
    {
        Task<bool> CreateTransactionAsync(Transaction transaction);
        Task<bool> UpdateBillAsync(Bill bill);
        Task<List<Doctor>> GetDoctorsDropdownAsync();
        Task<List<BillItem>> GetBillItemsByBillIdAsync(int billId);
        Task<List<Appointment>> GetAvailableAppointmentsAsync();
        Task<List<Appointment>> GetActiveAppointmentsByUserIdAsync(int userId);
        Task<Bill?> GetBillByAppointmentIdAsync(int appointmentId);
        Task<Appointment?> GetAppointmentByIdAsync(int appointmentId);
        Task<bool> BookAppointmentAsync(Appointment appointment, PROMIS10 promis10);
        Task<PROMIS10?> GetPROMIS10ByAppointmentIdAsync(int appointmentID);
        Task<bool> UpdateAppointmentAsync(Appointment appointment);
        Task<List<Bill>> GetPatientBillsAsync(int patientID);
    }
}
