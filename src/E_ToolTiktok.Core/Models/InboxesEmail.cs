namespace E_ToolTiktok.Core.Models;

public class InboxesEmail
{
    public string Email { get; set; } = string.Empty;
    public string InboxId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class InboxesMessage
{
    public string Id { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Preview { get; set; } = string.Empty;
    public DateTime Received { get; set; }
    public string? HtmlBody { get; set; }
    public string? TextBody { get; set; }
}

