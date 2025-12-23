using E_ToolTiktok.Core.Models;

namespace E_ToolTiktok.Core.Services;

public interface ITiktokApiService
{
    Task<RegistrationResponse> RegisterWithApiAsync(RegistrationRequest request);
    Task<bool> VerifyEmailWithApiAsync(string email, string verificationCode);
    Task<string?> GetCsrfTokenAsync();
}

