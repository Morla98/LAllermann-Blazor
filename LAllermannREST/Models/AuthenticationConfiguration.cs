namespace LAllermannREST.Models
{
    public class AuthenticationConfiguration
    {
        public string OwnerKey { get; set; } = string.Empty;
        public string JwtSecret { get; set; } = string.Empty;
        public string JwtIssuer { get; set; } = string.Empty;
        public string JwtAudience { get; set; } = string.Empty;
        public int JwtExpireTime { get; set; } = 1;
    }
}
