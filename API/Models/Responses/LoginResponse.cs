using API.Enums;
using API.Models.Base;

namespace API.Models.Responses
{
    public partial class LoginResponse
    {
        public AuthReason Reason { get; set; }
    }

    public partial class LoginResponse
    {
        public JWToken Token { get; set; }
    }
}
