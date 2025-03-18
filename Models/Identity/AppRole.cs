using Microsoft.AspNetCore.Identity;

namespace HospitalWebApplication.Models.Identity
{
    public class AppRole : IdentityRole<int>
    {
        public string? Description { get; set; }
    }
}
