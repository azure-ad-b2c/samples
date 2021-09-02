namespace password_history.Models
{
    public class ResponseContent
    {
        public string version { get; set; }
        public int status { get; set; }
        public string code { get; set; }
        public string userMessage { get; set; }
        public string developerMessage { get; set; }
        public string requestId { get; set; }
        public string moreInfo { get; set; }
    }
}