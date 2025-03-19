using HospitalWebApplication.Models.Doctors;
using HospitalWebApplication.Models.Interfaces;
using HospitalWebApplication.Models.Patients;
using HospitalWebApplication.Models;

namespace HospitalWebApplication.Services
{
    public class PatientService
    {
        private readonly IPatientRepository _patientRepository;

        public PatientService(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public async Task<List<Doctor>> GetDoctorsDropdownAsync()
        {
            return await _patientRepository.GetDoctorsDropdownAsync();
        }

        public async Task<List<Appointment>> GetAvailableAppointmentsAsync()
        {
            return await _patientRepository.GetAvailableAppointmentsAsync();
        }

        public async Task<List<BillItem>> GetBillItemsByBillIdAsync(int billId)
        {
            return await _patientRepository.GetBillItemsByBillIdAsync(billId);
        }

        public async Task<List<Appointment>> GetActiveAppointmentsByUserIdAsync(int userId)
        {
            var appointments = await _patientRepository.GetActiveAppointmentsByUserIdAsync(userId);

            // Attach bills to each appointment
            foreach (var appointment in appointments)
            {
                appointment.AttachedBill = await GetBillByAppointmentIdAsync(appointment.ID);
            }
            return appointments;
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int appointmentId)
        {
            return await _patientRepository.GetAppointmentByIdAsync(appointmentId);
        }

        public async Task<Bill?> GetBillByAppointmentIdAsync(int appointmentId)
        {
            return await _patientRepository.GetBillByAppointmentIdAsync(appointmentId);
        }

        public async Task<bool> BookAppointmentAsync(Appointment appointment, PROMIS10 promis10)
        {
            promis10.AppointmentId = appointment.ID;
            return await _patientRepository.BookAppointmentAsync(appointment, promis10);
        }

        public PROMIS10 ConvertListToPROMIS10(List<int> answers)
        {
            return new PROMIS10
            {
                Answer1 = answers[0],
                Answer2 = answers[1],
                Answer3 = answers[2],
                Answer4 = answers[3],
                Answer5 = answers[4],
                Answer6 = answers[5],
                Answer7 = answers[6]
            };
        }

        public async Task<bool> UpdateBillAsync(Bill bill)
        {
            return await _patientRepository.UpdateBillAsync(bill);
        }

        public async Task<PROMIS10?> GetPROMIS10ByAppointmentIdAsync(int appointmentID)
        {
            return await _patientRepository.GetPROMIS10ByAppointmentIdAsync(appointmentID);
        }

        public async Task<bool> UpdateAppointmentAsync(Appointment appointment)
        {
            return await _patientRepository.UpdateAppointmentAsync(appointment);
        }


        public async Task<bool> CreateTransactionAsync(Transaction transaction, int appointmentId)
        {
            var isCreated = await _patientRepository.CreateTransactionAsync(transaction);
            if (isCreated)
            {
                var appointment = await GetAppointmentByIdAsync(appointmentId);
                if (appointment != null)
                {
                    appointment.IsPaid = true;
                    return await _patientRepository.UpdateAppointmentAsync(appointment);
                }
            }
            return isCreated;
        }

        public async Task<List<Bill>> GetPatientBillsAsync(int patientID)
        {
            return await _patientRepository.GetPatientBillsAsync(patientID);
        }

        public async Task<List<Appointment>> GetAppointmentsByDoctorIdAsync(int doctorID)
        {
            var appointments = await _patientRepository.GetAvailableAppointmentsAsync();
            return appointments.Where(x => x.DoctorID == doctorID).ToList();
        }

    }
}
