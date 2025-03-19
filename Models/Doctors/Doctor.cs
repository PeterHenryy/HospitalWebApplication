using HospitalWebApplication.Models.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HospitalWebApplication.Models.Doctors
{
    public class Doctor
    {
        // Properties
        [Key]
        public int ID { get; set; }
        public string DoctorRole { get; set; }
        public int HierarchyStatus { get; set; }
        public string? Bio { get; set; }
        public string ProfilePictureURI { get; set; }
        public bool Active { get; set; } = true;

        // References
        [ForeignKey("AspNetUsers")]
        public int UserID { get; set; }
        public virtual AppUser User { get; set; }

    }
}
