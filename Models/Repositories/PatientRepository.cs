using HospitalWebApplication.Data;
using HospitalWebApplication.Models.Doctors;
using HospitalWebApplication.Models.Interfaces;
using HospitalWebApplication.Models.Patients;
using Microsoft.EntityFrameworkCore;

namespace HospitalWebApplication.Models.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _context;

        public PatientRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<bool> CreateTransactionAsync(Transaction transaction)
        {
            try
            {
                await _context.Transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateBillAsync(Bill bill)
        {
            try
            {
                _context.Bills.Update(bill);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Doctor>> GetDoctorsDropdownAsync()
        {
            return await _context.Doctors.Where(x => x.Active)
                                         .Include(x => x.User)
                                         .ToListAsync();
        }

        public async Task<List<BillItem>> GetBillItemsByBillIdAsync(int billId)
        {
            return await _context.BillItems.Where(x => x.BillId == billId)
                                           .ToListAsync();
        }

        public async Task<List<Appointment>> GetAvailableAppointmentsAsync()
        {
            return await _context.Appointments
                                 .Include(x => x.Doctor)
                                 .ThenInclude(x => x.User)
                                 .Include(x => x.User)
                                 .Include(x => x.AttachedBill)
                                 .ToListAsync();
        }

        public async Task<List<Appointment>> GetActiveAppointmentsByUserIdAsync(int userId)
        {
            return await _context.Appointments
                                 .Include(x => x.Doctor)
                                 .ThenInclude(x => x.User)
                                 .Include(x => x.User)
                                 .Include(x => x.AttachedBill)
                                 .Where(x => x.IsBooked && !x.IsPaid && x.UserID == userId)
                                 .ToListAsync();
        }

        public async Task<Bill?> GetBillByAppointmentIdAsync(int appointmentId)
        {
            return await _context.Bills
                                 .Include(x => x.Appointment)
                                 .FirstOrDefaultAsync(x => x.AppointmentId == appointmentId);
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int appointmentId)
        {
            return await _context.Appointments
                                 .Include(x => x.Doctor)
                                 .FirstOrDefaultAsync(x => x.ID == appointmentId);
        }

        public async Task<bool> BookAppointmentAsync(Appointment appointment, PROMIS10 promis10)
        {
            try
            {
                _context.Appointments.Update(appointment);
                await _context.PROMIS10s.AddAsync(promis10);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<PROMIS10?> GetPROMIS10ByAppointmentIdAsync(int appointmentID)
        {
            return await _context.PROMIS10s.SingleOrDefaultAsync(x => x.AppointmentId == appointmentID);
        }

        public async Task<bool> UpdateAppointmentAsync(Appointment appointment)
        {
            try
            {
                _context.Appointments.Update(appointment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Bill>> GetPatientBillsAsync(int patientID)
        {
            return await _context.Bills
                                 .Include(x => x.Appointment)
                                 .ThenInclude(x => x.Doctor)
                                 .ThenInclude(x => x.User)
                                 .Where(x => x.Appointment.UserID == patientID && x.Appointment.IsPaid)
                                 .ToListAsync();
        }

    }
}
