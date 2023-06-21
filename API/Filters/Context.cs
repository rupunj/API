using API.Const;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Filters
{
    public class Context
    {
        private protected object Respond { get; set; }
        private protected JResult WebRespond { get; set; }

        public Context(object respond)
        {
            Respond = respond;
        }

        public Context(dynamic webRespond, bool state = true)
        {
            Respond = new JResult() { State = state, Result = webRespond };
        }

        public ContentResult ToContextResult(int statusCode = Config.HTTP_STATUS_CODE_OK)
        {
            if (Respond.GetType().Name.Equals("JResult"))
                return new ContentResult { StatusCode = statusCode, ContentType = Config.CONTENT_TYPE_APPLICATION_JSON, Content = JsonConvert.SerializeObject(Respond) };
            else
            {
                if (!Respond.GetType().Namespace.StartsWith("System") || Respond.GetType().FullName.Contains("DataTable"))
                    return new ContentResult { StatusCode = statusCode, ContentType = Respond.GetType().FullName, Content = JsonConvert.SerializeObject(Respond) };
                else
                    return new ContentResult { StatusCode = statusCode, ContentType = Respond.GetType().FullName, Content = Respond.ToString() };
            }
        }
    }

    public class JResult
    {
        public bool State { get; set; } = true;
        public dynamic Result { get; set; }
    }
}
