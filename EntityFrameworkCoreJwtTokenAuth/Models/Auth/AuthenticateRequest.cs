using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkCoreJwtTokenAuth.Models.Auth
{
    public class AuthenticateRequest
    {
        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
