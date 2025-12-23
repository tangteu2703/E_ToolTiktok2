namespace E_ToolTiktok.Core.Models;

public class RegistrationRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string? PhoneNumber { get; set; }
    public string CountryCode { get; set; } = "VN"; // Mặc định Việt Nam
    // Mặc định không dùng proxy để tránh lỗi với các proxy miễn phí kém ổn định
    public bool UseProxy { get; set; } = false;
    public string? ProxyAddress { get; set; }
    public int? ProxyPort { get; set; }
    public string? ProxyUsername { get; set; }
    public string? ProxyPassword { get; set; }
    public string? ProxyType { get; set; } = "http"; // http, https, socks5
}

