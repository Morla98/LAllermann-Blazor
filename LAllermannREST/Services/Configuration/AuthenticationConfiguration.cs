namespace LAllermannREST.Services.Configuration
{
    public class AuthenticationConfiguration
    {
        public string OwnerKey { get; set; } = String.Empty;
        public string JwtSecret { get; set; } = String.Empty;
        public string JwtIssuer { get; set; } = String.Empty;
        public string JwtAudience { get; set; } = String.Empty;
        public int JwtExpireTime { get; set; } = 1;
    }
}
