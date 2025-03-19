namespace HospitalWebApplication.Models.Interfaces
{
    public interface IAdminRepository
    {
        Task<List<Bill>> GetPaidBillsAsync();
        Task<List<Bill>> GetBillsByDoctorIDAsync(int doctorID);
        Task<decimal> CalculateDoctorRevenueAsync(int doctorID);
        Task<decimal> CalculateHospitalRevenueAsync();
    }
}
