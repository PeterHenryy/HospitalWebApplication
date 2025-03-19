using HospitalWebApplication.Models.Doctors;
using HospitalWebApplication.Models.Patients;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HospitalWebApplication.Models.Admins.ViewModels
{
    public class AppointmentDetailsViewModel
    {
        public Appointment Appointment { get; set; }
        public int AppointmentID { get; set; }
        public int DoctorID { get; set; }
        public Bill Bill { get; set; }
        public PROMIS10 PROMIS10 { get; set; }
        public List<BillItem> BillItemsAdded { get; set; }
        public List<Doctor> Doctors { get; set; }

    }
}
