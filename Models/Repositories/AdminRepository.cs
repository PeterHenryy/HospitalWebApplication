using HospitalWebApplication.Data;
using HospitalWebApplication.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalWebApplication.Models.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Bill>> GetPaidBillsAsync()
        {
            return await _context.Bills
                .Include(x => x.Appointment)
                .Where(x => x.Appointment.IsPaid)
                .ToListAsync();
        }

        public async Task<List<Bill>> GetBillsByDoctorIDAsync(int doctorID)
        {
            return await _context.Bills
                .Include(x => x.Appointment)
                .Where(x => x.Appointment.DoctorID == doctorID)
                .ToListAsync();
        }

        public async Task<decimal> CalculateDoctorRevenueAsync(int doctorID)
        {
            var doctorBills = await GetBillsByDoctorIDAsync(doctorID);
            return doctorBills.Where(x => x.Appointment.IsPaid).Sum(x => x.OriginalTotal);
        }

        public async Task<decimal> CalculateHospitalRevenueAsync()
        {
            var paidBills = await GetPaidBillsAsync();
            return paidBills.Sum(x => x.OriginalTotal);
        }
    }
}
