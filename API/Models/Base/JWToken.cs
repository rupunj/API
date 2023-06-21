using System.ComponentModel.DataAnnotations;

namespace API.Models.Base
{
    public partial class JWToken 
    {
        [Required]
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public partial class JWToken
    {
        public string Scope { get; set; }
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; }
    }
}
