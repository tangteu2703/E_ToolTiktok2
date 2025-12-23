namespace E_ToolTiktok.Core.Models;

public class RegistrationRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string? PhoneNumber { get; set; }
    public string CountryCode { get; set; } = "VN"; // Mặc định Việt Nam
    public bool UseProxy { get; set; } = true; // Mặc định dùng proxy
    public string? ProxyAddress { get; set; }
    public int? ProxyPort { get; set; }
    public string? ProxyUsername { get; set; }
    public string? ProxyPassword { get; set; }
    public string? ProxyType { get; set; } = "http"; // http, https, socks5
}

