namespace katlog_backend.Settings;

public class JwtSettings
{
    public required string SecretKey { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public double ExpiryInDays { get; set; }
}