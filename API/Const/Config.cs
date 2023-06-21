namespace API.Const
{
    public static class Config
    {
        public const int HTTP_STATUS_CODE_OK = 200;
        public const int STATUS_CODE_FORBIDDEN = 403;
        public const int HTTP_STATUS_CODE_BAD = 400;

        public const string APPLICATION_NAME = "cart.api";

        public const string AUTHORIZATION_TYPE_BEARER = "bearer";
        public const string AUTHORIZATION_TYPE_BASIC = "basic";

        public const string CONTENT_TYPE_APPLICATION_JSON = "application/json";
        public const string CONTENT_TYPE_APPLICATION_URLENCODED = "application/x-www-form-urlencoded";

        public const string TRACE_CUSTOME_HEADER = "X-Correlation-ID";
    }
}
