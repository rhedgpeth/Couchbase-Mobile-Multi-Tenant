namespace AuthApi.Core.Models
{
    public class Session
    {
        public string cookie_name { get; set; }
        public string expires { get; set; }
        public string session_id { get; set; }

        public string TenantId { get; set; } 
    }
}
