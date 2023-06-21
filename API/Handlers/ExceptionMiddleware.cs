using API.Const;
using API.Models.Responses;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using API.Models.Base;

namespace API.Handlers
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Settings _config;
        private ILogger Log { get; set; }

        public ExceptionMiddleware(RequestDelegate next, IOptions<Settings> config)
        {
            _next = next;
            _config = config.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (!context.Response.HasStarted)
                {
                    while (ex.InnerException != null) ex = ex.InnerException;


                    try
                    {
                        #region Construct Initials

                        bool customized = false;
                        string customizedError = "";
                        int haddledStatusCode = Config.HTTP_STATUS_CODE_BAD;

                        if (ex.Message != null && ex.Message.Contains(ErrorStatus.STATUS_CUSTOMIZED_ERROR, StringComparison.InvariantCultureIgnoreCase))
                        {
                            customized = true;
                            customizedError = ex.Message.Replace(ErrorStatus.STATUS_CUSTOMIZED_ERROR, "");
                        }

                        ErrorResponse respond = new();
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
                            Guid GUI = Guid.NewGuid();
                            ID = Guid.NewGuid().ToString().Replace("-", "");

                            if (ex.Message != null)
                            {
                                ErrorStatus status = ErrorStatus.Instance;
                                var result = status.StatusList.Find(code => code.Key.Contains(ex.Message, StringComparison.InvariantCultureIgnoreCase));

                                if (result.Key != null)
                                    haddledStatusCode = result.Value;

                                respond = new ErrorResponse
                                {
                                    Error = ex.Message,
                                    ErrorID = ID
                                };
                            }
                        }

                        context.Response.Headers.Add(Config.TRACE_CUSTOME_HEADER, context.TraceIdentifier);
                        context.Response.Headers.Add("Content-Type", Config.CONTENT_TYPE_APPLICATION_JSON);
                        context.Response.StatusCode = haddledStatusCode;
                        await context.Response.WriteAsync(haddledStatusCode == 401 ? "" : JsonConvert.SerializeObject(respond));

                        #endregion
                    }
                    catch
                    {
                        int haddledStatusCode = Config.HTTP_STATUS_CODE_BAD;

                        ErrorStatus status = ErrorStatus.Instance;
                        var result = status.StatusList.Find(code => code.Key.Contains(ErrorStatus.STATUS_MSG_UNSUPPORTED_CONTENT, StringComparison.InvariantCultureIgnoreCase));

                        if (result.Key != null)
                        {
                            haddledStatusCode = result.Value;
                        }

                        context.Response.Headers.Add(Config.TRACE_CUSTOME_HEADER, context.TraceIdentifier);
                        context.Response.Headers.Add("Content-Type", Config.CONTENT_TYPE_APPLICATION_JSON);
                        context.Response.StatusCode = haddledStatusCode;
                        await context.Response.WriteAsync("");
                    }
                }
            }
        }
    }
}
