using HospitalWebApplication.Models.Interfaces;

namespace HospitalWebApplication.Services
{
    public class AdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<decimal> CalculateDoctorRevenueAsync(int doctorID)
        {
            return await _adminRepository.CalculateDoctorRevenueAsync(doctorID);
        }

        public async Task<decimal> CalculateHospitalRevenueAsync()
        {
            return await _adminRepository.CalculateHospitalRevenueAsync();
        }

        public static bool HasOccurred(DateTime eventDateTime)
        {
            return eventDateTime <= DateTime.Now;
        }
    }
}
