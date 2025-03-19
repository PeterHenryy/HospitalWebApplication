using HospitalWebApplication.Models.Doctors;
using HospitalWebApplication.Models.Identity;
using HospitalWebApplication.Models.Patients;

namespace HospitalWebApplication.Models.Interfaces
{
    public interface IDoctorRepository
    {
        Task<PROMIS10> GetPROMIS10ByAppointmentIDAsync(int appointmentID);
        Task<bool> CreateAsync(Doctor doctor);
        Task<bool> RemoveAsync(int doctorID);
        Task<List<Appointment>> GetAppointmentsByDoctorAsync(int doctorId);
        Task<List<AppUser>> GetAllPatientsAsync();
        Task<List<Doctor>> GetAllDoctorsAsync();
        Task<Doctor> GetDoctorByIDAsync(int doctorID);
        Task<Bill> GetBillByIdAsync(int billId);
        Task<Appointment> GetAppointmentByIdAsync(int appointmentId);
        Task<List<BillItem>> GetBillItemsByBillIdAsync(int billId);
        Task<bool> CreateAppointmentAsync(Appointment appointment);
        Task<bool> CreateBillItemsAsync(BillItem bill);
        Task<bool> UpdateBillAsync(Bill bill);
        Task<bool> UpdateDoctorAsync(Doctor doctor);
        Task<Bill> GetUpdatedBillByAppointmentIdAsync(int appointmentId);
        Task<Bill> CreateBillAsync(Bill bill);
        Task<Doctor> GetDoctorByUserIDAsync(int userID);
        Task RejectAppointmentAsync(Appointment appointment);
        Task<BillItem> GetBillItemByIDAsync(int billItemID);
        Task<bool> RemoveBillItemAsync(int billItemID);
    }
}
