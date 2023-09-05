using System.ComponentModel.DataAnnotations;

namespace Core.Application.Models.Identity
{
    public class RegistrationRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
