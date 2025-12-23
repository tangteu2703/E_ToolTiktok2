using E_ToolTiktok.Core.Models;

namespace E_ToolTiktok.Core.Services;

public interface ITiktokRegistrationService
{
    Task<RegistrationResponse> RegisterAccountAsync(RegistrationRequest request);
    Task<bool> VerifyEmailAsync(string email, string verificationCode);
    Task<bool> VerifyPhoneAsync(string phoneNumber, string verificationCode);
}

