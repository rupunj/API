using API.Const;
using API.Enums;
using API.Interfaces;
using API.Models.Base;

namespace API.Services
{
    public class IDP : IGranter
    {
        private static readonly string TOKEN_REQUEST_ENDPOINT = "/connect/token";
        private static readonly string TOKEN_VERIFY_ENDPOINT = "/connect/introspect";
        private static readonly string TOKEN_RECVOCATION_ENDPOINT = "/connect/revocation";
        private protected Service Provider { get; set; }

        public IDP(Service config)
        {
            Provider = config;
        }

        public async Task<(bool, string)> Token(string basicToken, string username, string password, string audience, List<string> scopes)
        {
            List<KeyValuePair<string, string>> collection = new()
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("audience", audience),
                new KeyValuePair<string, string>("scope", string.Join(" ", scopes.ToArray()) )
            };

            RestClient restCon = new();
            return await restCon.PostAPIInvoke(Provider.BaseURL, TOKEN_REQUEST_ENDPOINT, collection, basicToken, Config.AUTHORIZATION_TYPE_BASIC);
        }
        public async Task<(bool, string)> VerifyToken(string basicToken, string token)
        {
            List<KeyValuePair<string, string>> collection = new()
            {
                new KeyValuePair<string, string>("token", token)
            };

            RestClient restCon = new();
            return await restCon.PostAPIInvoke(Provider.BaseURL, TOKEN_VERIFY_ENDPOINT, collection, basicToken, Config.AUTHORIZATION_TYPE_BASIC);
        }
        public async Task<(bool, string)> RefreshToken(string clientID, string clientSecret, string refreshToken)
        {
            List<KeyValuePair<string, string>> collection = new()
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("client_id", clientID),
                new KeyValuePair<string, string>("client_secret", clientSecret)
            };

            RestClient restCon = new();
            return await restCon.PostAPIInvoke(Provider.BaseURL, TOKEN_REQUEST_ENDPOINT, collection, "", Config.AUTHORIZATION_TYPE_BASIC);
        }
        public async Task<(bool, string)> Revocation(string basicToken, string token, TokenType tokenType)
        {
            List<KeyValuePair<string, string>> collection = new()
            {
                new KeyValuePair<string, string>("token", token),
                new KeyValuePair<string, string>("token_type_hint", tokenType.ToString())
            };

            RestClient restCon = new();
            return await restCon.PostAPIInvoke(Provider.BaseURL, TOKEN_RECVOCATION_ENDPOINT, collection, basicToken, Config.AUTHORIZATION_TYPE_BASIC);
        }
    }
}
