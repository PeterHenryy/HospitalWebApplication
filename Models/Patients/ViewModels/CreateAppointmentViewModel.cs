using HospitalWebApplication.Models.Doctors;

namespace HospitalWebApplication.Models.Patients.ViewModels
{
    public class CreateAppointmentViewModel
    {
        public Appointment Appointment { get; set; }
        public List<Doctor> Doctors { get; set; }
    }
}
