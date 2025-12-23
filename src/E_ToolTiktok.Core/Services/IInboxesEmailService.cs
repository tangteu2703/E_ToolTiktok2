using E_ToolTiktok.Core.Models;

namespace E_ToolTiktok.Core.Services;

public interface IInboxesEmailService
{
    Task<InboxesEmail> CreateRandomEmailAsync();
    Task<List<InboxesMessage>> GetMessagesAsync(string email);
    Task<string?> GetVerificationCodeAsync(string email, int timeoutSeconds = 60);
    Task<InboxesMessage?> WaitForMessageAsync(string email, string fromContains = "tiktok", int timeoutSeconds = 60);
}

