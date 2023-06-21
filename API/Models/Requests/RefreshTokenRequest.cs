using System.ComponentModel.DataAnnotations;

namespace API.Models.Requests
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
