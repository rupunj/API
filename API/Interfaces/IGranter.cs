using API.Enums;

namespace API.Interfaces
{
    public interface IGranter
    {
        public Task<(bool, string)> Token(string authToken, string username, string password, string audience, List<string> scopes);
        public Task<(bool, string)> VerifyToken(string basicToken, string token);
        public Task<(bool, string)> RefreshToken(string clientID, string clientSecret, string refreshToken);
        public Task<(bool, string)> Revocation(string basicToken, string token, TokenType tokenType);
    }
}
