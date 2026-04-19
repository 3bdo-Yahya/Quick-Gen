namespace Quick_Gen.Configuration;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "QuickGen";
    public string Audience { get; set; } = "QuickGenClients";
    public string Key { get; set; } = string.Empty;
    public int ExpiresHours { get; set; } = 72;
}
