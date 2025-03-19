using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HospitalWebApplication.Models.Patients.ViewModels
{
    public class BookAppointmentViewModelForm
    {
        public Appointment AppointmentDataForm { get; set; }
        public PROMIS10 PROMIS10DataForm { get; set; }

    }
}
