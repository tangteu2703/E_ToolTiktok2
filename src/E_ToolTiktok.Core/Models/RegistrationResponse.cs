namespace E_ToolTiktok.Core.Models;

public class RegistrationResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public TiktokAccount? Account { get; set; }
    public string? ErrorCode { get; set; }
    public Dictionary<string, object>? AdditionalData { get; set; }
}

