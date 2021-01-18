using Microsoft.AspNetCore.Identity;

namespace API.Models
{
    public class User:IdentityUser
    {
        public int Id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string mobileNumber { get; set; }
        public string resetPasswordCode { get; set; }
    }
}

