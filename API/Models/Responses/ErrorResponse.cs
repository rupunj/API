namespace API.Models.Responses
{
    public class ErrorResponse
    {
        public string ErrorID { get; set; }
        public string Error { get; set; }
        public int StatusCode { get; set; }
    }
}
