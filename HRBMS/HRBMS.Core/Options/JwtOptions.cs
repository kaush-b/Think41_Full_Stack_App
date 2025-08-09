namespace HRBMS.Core.Options;

public class JwtOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty; // Symmetric key
    public int ExpiryMinutes { get; set; } = 60;
}