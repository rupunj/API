using API.Models.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;
using System.Text;
using API.Interfaces;
using API.Services;
using API.Models.Responses;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using API.Const;
using API.Enums;
using API.Filters;
using API.Models.Requests;

namespace API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class GranterController : Controller
    {
        private readonly IConfiguration config;
        private readonly Settings settings;

        public GranterController(IConfiguration _configuration, IOptions<Settings> _settings)
        {
            config = _configuration;
            settings = _settings.Value;
        }

        [HttpPost, Route("Login")]
        public async Task<IActionResult> Login_1()
        {
            string basicAuth = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            string authID = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(basicAuth)).Split(":")[0];
            string authPass = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(basicAuth)).Split(":")[1];

            IDataAccess db = ServiceInit.GetDataInstance(settings.DataAccess);

            string authToken = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(settings.Client.ID + ":" + Convert.ToBase64String(Encoding.UTF8.GetBytes(settings.Client.ActivationKey.Replace("+", "")))));

            LoginResponse respond = new();

            IGranter granter = ServiceInit.GetIDPInstance(settings.IDP);
            DataTable userScopes = await db.GetUserScopes(authID);

            List<string> scopeList = userScopes.AsEnumerable().Select(x => x["Scope"].ToString()).ToList();

            //List<string> scopeList = new List<string>();
            //scopeList.Add("Create");
            //scopeList.Add("Edit");
            //scopeList.Add("View");

            if (scopeList != null && settings.IDP.LongLivedToken.Enable)
                scopeList.Add("offline_access");
            else if (settings.IDP.LongLivedToken.Enable)
                scopeList = new List<string>() { "offline_access" };

            (bool, string) tokenResult = await granter.Token(authToken, authID, authPass, Config.APPLICATION_NAME, scopeList);

            if (tokenResult.Item1)
            {
                JObject tokenObject = JObject.Parse(tokenResult.Item2);

                JWToken token = new()
                {
                    AccessToken = tokenObject["access_token"].ToString(),
                    Scope = tokenObject["scope"].ToString(),
                    ExpiresIn = Convert.ToInt32(tokenObject["expires_in"].ToString()),
                    TokenType = tokenObject["token_type"].ToString(),
                    RefreshToken = tokenObject.ContainsKey("refresh_token") ? tokenObject["refresh_token"].ToString() : null
                };

                respond.Reason = AuthReason.Authenticated;
                respond.Token = token;

                return new Context(respond).ToContextResult();
            }
            else
            {
                if (tokenResult.Item2.Equals("Unauthorized"))
                    return new Context("Insufficient Privilege").ToContextResult(Config.STATUS_CODE_FORBIDDEN);

                JObject error = JsonConvert.DeserializeObject<JObject>(tokenResult.Item2);

                if (error != null && error["error"].ToString().Equals("unauthorized_client") && error["error_description"].ToString().Equals("invalid_username_or_password"))
                    respond.Reason = (AuthReason)Convert.ToInt32(error["Reason"]);
                else
                    throw new Exception(tokenResult.Item2);

                return new Context(respond).ToContextResult();
            }
        }

        [HttpPost, Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken_1([FromBody] RefreshTokenRequest context)
        {
            if (!TryValidateModel(context))
            {
                string errors = "";
                if (ModelState.ErrorCount > 0)
                    errors = JsonConvert.SerializeObject(ModelState.Where(val => val.Value.Errors.Count > 0).Select(val => new { val.Key, val.Value.Errors }).ToArray());

                throw new Exception(string.Concat(ErrorStatus.STATUS_MSG_MODEL_INVALID, errors));
            }

            string basicAuth = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            string authID = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(basicAuth)).Split(":")[0];


            IGranter granter = ServiceInit.GetIDPInstance(settings.IDP);
            (bool, string) tokenResult = await granter.RefreshToken(settings.Client.ID, Convert.ToBase64String(Encoding.UTF8.GetBytes(settings.Client.ActivationKey.Replace("+", ""))), context.RefreshToken);

            if (!tokenResult.Item1)
                if (tokenResult.Item2.Equals("Unauthorized"))
                    throw new Exception(string.Concat(ErrorStatus.STATUS_MSG_NOT_PERMITTED, "Insufficient Privilege"));
                else
                    throw new Exception(ErrorStatus.STATUS_MSG_NOT_PERMITTED);

            JObject tokenObject = JsonConvert.DeserializeObject<JObject>(tokenResult.Item2);

            LoginResponse respond = new()
            {
                Token = new()
                {
                    AccessToken = tokenObject["access_token"].ToString(),
                    Scope = tokenObject["scope"].ToString(),
                    ExpiresIn = Convert.ToInt32(tokenObject["expires_in"].ToString()),
                    TokenType = tokenObject["token_type"].ToString(),
                    RefreshToken = tokenObject.ContainsKey("refresh_token") ? tokenObject["refresh_token"].ToString() : null
                }
            };
            
            return new Context(respond).ToContextResult();
        }
    }
}
