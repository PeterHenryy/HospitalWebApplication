using HospitalWebApplication.Data;
using HospitalWebApplication.Models.Doctors;
using HospitalWebApplication.Models.Identity;
using HospitalWebApplication.Models.Interfaces;
using HospitalWebApplication.Models.Patients;
using Microsoft.EntityFrameworkCore;

namespace HospitalWebApplication.Models.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PROMIS10> GetPROMIS10ByAppointmentIDAsync(int appointmentID)
        {
            return await _context.PROMIS10s.SingleOrDefaultAsync(x => x.AppointmentId == appointmentID);
        }

        public async Task<bool> CreateAsync(Doctor doctor)
        {
            try
            {
                await _context.Doctors.AddAsync(doctor);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveAsync(int doctorID)
        {
            var doctor = await GetDoctorByIDAsync(doctorID);
            if (doctor == null) return false;

            doctor.Active = false;
            return await UpdateDoctorAsync(doctor);
        }

        public async Task<List<Appointment>> GetAppointmentsByDoctorAsync(int doctorId)
        {
            return await _context.Appointments
                .Where(x => x.Doctor.ID == doctorId)
                .Include(x => x.User)
                .Include(x => x.AttachedBill)
                .ToListAsync();
        }

        public async Task<List<AppUser>> GetAllPatientsAsync()
        {
            return await _context.Appointments
                .Select(x => x.User)
                .Distinct()
                .Where(x => x != null)
                .ToListAsync();
        }

        public async Task<List<Doctor>> GetAllDoctorsAsync()
        {
            return await _context.Doctors
                .Where(x => x.Active)
                .Include(x => x.User)
                .ToListAsync();
        }

        public async Task<Doctor> GetDoctorByIDAsync(int doctorID)
        {
            return await _context.Doctors
                .Include(x => x.User)
                .SingleOrDefaultAsync(x => x.ID == doctorID);
        }

        public async Task<Bill> GetBillByIdAsync(int billId)
        {
            return await _context.Bills
                .Include(x => x.Appointment)
                .SingleOrDefaultAsync(x => x.Id == billId);
        }

        public async Task<Appointment> GetAppointmentByIdAsync(int appointmentId)
        {
            return await _context.Appointments
                .Include(x => x.Doctor)
                .SingleOrDefaultAsync(x => x.ID == appointmentId);
        }

        public async Task<List<BillItem>> GetBillItemsByBillIdAsync(int billId)
        {
            return await _context.BillItems
                .Where(x => x.BillId == billId)
                .ToListAsync();
        }

        public async Task<bool> CreateAppointmentAsync(Appointment appointment)
        {
            try
            {
                await _context.Appointments.AddAsync(appointment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreateBillItemsAsync(BillItem bill)
        {
            try
            {
                await _context.BillItems.AddAsync(bill);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
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
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateDoctorAsync(Doctor doctor)
        {
            try
            {
                _context.Doctors.Update(doctor);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Bill> GetUpdatedBillByAppointmentIdAsync(int appointmentId)
        {
            return await _context.Bills
                .Include(x => x.Appointment)
                .FirstOrDefaultAsync(x => x.AppointmentId == appointmentId);
        }

        public async Task<Bill> CreateBillAsync(Bill bill)
        {
            try
            {
                await _context.Bills.AddAsync(bill);
                await _context.SaveChangesAsync();
                return bill;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Doctor> GetDoctorByUserIDAsync(int userID)
        {
            return await _context.Doctors
                .SingleOrDefaultAsync(x => x.UserID == userID);
        }

        public async Task RejectAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task<BillItem> GetBillItemByIDAsync(int billItemID)
        {
            return await _context.BillItems
                .SingleOrDefaultAsync(x => x.Id == billItemID);
        }

        public async Task<bool> RemoveBillItemAsync(int billItemID)
        {
            try
            {
                var billItem = await GetBillItemByIDAsync(billItemID);
                if (billItem == null) return false;

                _context.BillItems.Remove(billItem);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
