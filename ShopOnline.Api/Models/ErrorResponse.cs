namespace ShopOnline.Api.Models
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public DateTime Timestamp { get; set; }
        public string Path { get; set; }

        public ErrorResponse()
        {
            /* Get the date and time for the current moment expressed
            ** as coordinated universal time (UTC).
            ** DateTime.Now => adjusted to the local time zone.*/
            Timestamp = DateTime.UtcNow;
        }
        public ErrorResponse(int statusCode, 
                             string message, 
                             string details = null, 
                             string path = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
            Path = path;
            Timestamp = DateTime.UtcNow;
        }
    }
}
