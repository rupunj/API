using API.Const;
using API.Models.Responses;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly IConfiguration config;
        private ILogger Log { get; set; }

        public ExceptionFilter(IConfiguration configuration)
        {
            config = configuration;
        }

        public void OnException(ExceptionContext context)
        {
            #region Construct Initials

            bool customized = false;
            string customizedError = "";

            while (context.Exception.InnerException != null) context.Exception = context.Exception.InnerException;

            if (context.Exception.Message != null && context.Exception.Message.Contains(ErrorStatus.STATUS_CUSTOMIZED_ERROR, StringComparison.InvariantCultureIgnoreCase))
            {
                customized = true;
                customizedError = context.Exception.Message.Replace(ErrorStatus.STATUS_CUSTOMIZED_ERROR, "");
            }

            ErrorResponse respond = new();
            int haddledStatusCode = Config.HTTP_STATUS_CODE_BAD;
            string ID = "";

            if (customized)
            {
                respond = new ErrorResponse
                {
                    Error = customizedError,
                    ErrorID = customizedError.Split("#")[1]
                };

                ID = customizedError.Split("#")[1];
            }
            else
            {
                ID = Guid.NewGuid().ToString().Replace("-", "");

                respond = new ErrorResponse
                {
                    Error = context.Exception.Message,
                    ErrorID = ID
                };

                if (context.Exception.Message != null)
                {
                    ErrorStatus status = ErrorStatus.Instance;
                    var result = status.StatusList.Find(code => code.Key.Contains(context.Exception.Message, StringComparison.InvariantCultureIgnoreCase));

                    if (result.Key != null)
                    {
                        haddledStatusCode = result.Value;
                    }
                }
            }

            #endregion

            context.HttpContext.Response.Headers.Add(Config.TRACE_CUSTOME_HEADER, context.HttpContext.TraceIdentifier);
            context.Result = new ContentResult { StatusCode = haddledStatusCode, Content = haddledStatusCode == 401 ? "" : JsonConvert.SerializeObject(respond) };
        }        
    }
}
