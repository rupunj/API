namespace API.Const
{
    public sealed class ErrorStatus
    {
        private ErrorStatus()
        {
            initStatusList();
        }
        private static ErrorStatus instance = null;
        public static ErrorStatus Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ErrorStatus();
                }
                return instance;
            }
        }

        public List<KeyValuePair<string, int>> StatusList { get; set; }

        // Status Codes
        private const int STATUS_CODE_BAD_REQUEST = 400;
        private const int STATUS_CODE_UNAUTHORIZED = 401;
        private const int STATUS_CODE_FORBIDDEN = 403;
        private const int STATUS_CODE_PRE_CONDITION_FAIL = 412;
        private const int STATUS_CODE_CERT_EXPIRED = 480;
        private const int STATUS_CODE_CERT_VERIFICATION_FAIL = 481;
        private const int STATUS_CODE_CERT_REVOKED = 482;
        private const int STATUS_CODE_REQUEST_INVALID = 491;
        private const int STATUS_CODE_UNACCEPTABLE_KEY = 494;
        private const int STATUS_CODE_UNACCEPTABLE_ALGORITHM = 495;
        private const int STATUS_CODE_UNACCEPTABLE_CONTENT = 498;
        private const int STATUS_CODE_CLIENT_NOT_UNACCEPTABLE = 499;
        private const int STATUS_CODE_BLOCK = 423;


        //Status Messages
        public const string STATUS_MSG_INVLID_KEY = "Invalid Key";
        public const string STATUS_MSG_KEY_MISMATCH = "Authorization Key Mismatch";
        public const string STATUS_MSG_MODEL_INVALID = "Model Invalid";
        public const string STATUS_MSG_UNPROTECTED = "Content Not Protected";
        public const string STATUS_MSG_EMPTY = "Empty Content";
        public const string STATUS_MSG_SIGANATURE_FAIL = "Signature Verification Fail";
        public const string STATUS_MSG_PROTECTED = "Content Protected";
        public const string STATUS_MSG_CERT_NOT_FOUND = "Public Certification Not Found";
        public const string STATUS_MSG_CERT_CHAIN_FAIL = "Certificate Validation Fail";
        public const string STATUS_MSG_CERT_REVOKED = "Certificate Revoked";
        public const string STATUS_MSG_CERT_EXPIRED = "Certificate Expired";
        public const string STATUS_MSG_INVALID_RESPONSE_TYPE = "Inavllid Response Type";
        public const string STATUS_MSG_UNSUPPORTED_ENCRYPTION = "Un-Supported Encryption Protocol";
        public const string STATUS_MSG_UNSUPPORTED_PUSH_PLATFORM = "Un-Supported Push Platform";
        public const string STATUS_MSG_TOKEN_EXPIRED = "Access Token Expired";
        public const string STATUS_CUSTOMIZED_ERROR = "Customized Error:";
        public const string STATUS_MSG_NOT_PERMITTED = "Forbidden Action";
        public const string STATUS_MSG_UNSUPPORTED_CONTENT = "Un-Supported Content";
        public const string STATUS_MSG_USER_EXIST = "User Exsist";
        public const string STATUS_MSG_CLIENT_INVALID = "Client Configuration Signature Fail";
        public const string STATUS_MSG_CLIENT_FAIL = "Build Signature Fail";
        public const string STATUS_MSG_EXSIST_CSR_FAIL = "CSR Data Mismatch";
        public const string STATUS_MSG_CSR_FAIL = "CSR Fail";
        public const string STATUS_MSG_TOO_MANY_OTP_REQUEST = "Too Many OTP Request";
        public const string STATUS_MSG_IN_PROGRESS = "Work In Progress";

        private void initStatusList()
        {
            StatusList = new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>(STATUS_MSG_INVLID_KEY, STATUS_CODE_REQUEST_INVALID),
                new KeyValuePair<string, int>(STATUS_MSG_KEY_MISMATCH, STATUS_CODE_UNACCEPTABLE_CONTENT),
                new KeyValuePair<string, int>(STATUS_MSG_MODEL_INVALID, STATUS_CODE_UNACCEPTABLE_CONTENT),
                new KeyValuePair<string, int>(STATUS_MSG_UNPROTECTED, STATUS_CODE_UNACCEPTABLE_KEY),
                new KeyValuePair<string, int>(STATUS_MSG_EMPTY, STATUS_CODE_BAD_REQUEST),
                new KeyValuePair<string, int>(STATUS_MSG_SIGANATURE_FAIL, STATUS_CODE_BAD_REQUEST),
                new KeyValuePair<string, int>(STATUS_MSG_PROTECTED, STATUS_CODE_BAD_REQUEST),
                new KeyValuePair<string, int>(STATUS_MSG_CERT_NOT_FOUND, STATUS_CODE_PRE_CONDITION_FAIL),
                new KeyValuePair<string, int>(STATUS_MSG_CERT_CHAIN_FAIL, STATUS_CODE_CERT_VERIFICATION_FAIL),
                new KeyValuePair<string, int>(STATUS_MSG_CERT_REVOKED, STATUS_CODE_CERT_REVOKED),
                new KeyValuePair<string, int>(STATUS_MSG_CERT_EXPIRED, STATUS_CODE_CERT_EXPIRED),
                new KeyValuePair<string, int>(STATUS_MSG_INVALID_RESPONSE_TYPE, STATUS_CODE_UNACCEPTABLE_CONTENT),
                new KeyValuePair<string, int>(STATUS_MSG_UNSUPPORTED_ENCRYPTION, STATUS_CODE_UNACCEPTABLE_ALGORITHM),
                new KeyValuePair<string, int>(STATUS_MSG_UNSUPPORTED_PUSH_PLATFORM, STATUS_CODE_UNACCEPTABLE_CONTENT),
                new KeyValuePair<string, int>(STATUS_MSG_TOKEN_EXPIRED, STATUS_CODE_UNAUTHORIZED),
                new KeyValuePair<string, int>(STATUS_MSG_NOT_PERMITTED, STATUS_CODE_FORBIDDEN),
                new KeyValuePair<string, int>(STATUS_MSG_UNSUPPORTED_CONTENT, STATUS_CODE_UNACCEPTABLE_CONTENT),
                new KeyValuePair<string, int>(STATUS_MSG_USER_EXIST, STATUS_CODE_REQUEST_INVALID),
                new KeyValuePair<string, int>(STATUS_MSG_CLIENT_INVALID, STATUS_CODE_BAD_REQUEST),
                new KeyValuePair<string, int>(STATUS_MSG_CLIENT_FAIL, STATUS_CODE_CLIENT_NOT_UNACCEPTABLE),
                new KeyValuePair<string, int>(STATUS_MSG_EXSIST_CSR_FAIL, STATUS_CODE_UNACCEPTABLE_CONTENT),
                new KeyValuePair<string, int>(STATUS_MSG_CSR_FAIL, STATUS_CODE_BAD_REQUEST),
                new KeyValuePair<string, int>(STATUS_MSG_TOO_MANY_OTP_REQUEST, STATUS_CODE_FORBIDDEN),
                new KeyValuePair<string, int>(STATUS_MSG_IN_PROGRESS, STATUS_CODE_BLOCK)
            };
        }
    }
}
