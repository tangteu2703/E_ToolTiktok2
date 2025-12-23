namespace E_ToolTiktok.Core.Models;

public class TiktokAccount
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsVerified { get; set; }
    public string? SessionId { get; set; }
}

