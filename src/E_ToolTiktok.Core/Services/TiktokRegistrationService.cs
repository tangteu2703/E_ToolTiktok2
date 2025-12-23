using E_ToolTiktok.Core.Models;

namespace E_ToolTiktok.Core.Services;

public class TiktokRegistrationService : ITiktokRegistrationService
{
    private readonly ITiktokApiService _tiktokApiService;
    private readonly IInboxesEmailService _inboxesEmailService;
    private readonly INameGeneratorService _nameGeneratorService;
    private readonly IProxyService _proxyService;

    public TiktokRegistrationService(
        ITiktokApiService tiktokApiService,
        IInboxesEmailService inboxesEmailService,
        INameGeneratorService nameGeneratorService,
        IProxyService proxyService)
    {
        _tiktokApiService = tiktokApiService;
        _inboxesEmailService = inboxesEmailService;
        _nameGeneratorService = nameGeneratorService;
        _proxyService = proxyService;
    }

    public async Task<RegistrationResponse> RegisterAccountAsync(RegistrationRequest request)
    {
        try
        {
            // Xác định country code
            var countryCode = string.IsNullOrEmpty(request.CountryCode) ? "VN" : request.CountryCode.ToUpper();
            var country = Countries.GetByCode(countryCode) ?? Countries.GetByCode("VN");

            // Nếu không có email, tạo email ngẫu nhiên từ inboxes.com
            if (string.IsNullOrEmpty(request.Email))
            {
                var inboxesEmail = await _inboxesEmailService.CreateRandomEmailAsync();
                request.Email = inboxesEmail.Email;
            }

            // Tạo username theo quốc gia nếu chưa có
            if (string.IsNullOrEmpty(request.Username))
            {
                request.Username = await _nameGeneratorService.GenerateUsernameAsync(countryCode, request.Email);
            }

            // Lấy proxy theo quốc gia nếu UseProxy = true
            if (request.UseProxy)
            {
                var proxy = await _proxyService.GetProxyAsync(countryCode);
                if (proxy != null)
                {
                    request.ProxyAddress = proxy.Host;
                    request.ProxyPort = proxy.Port;
                    request.ProxyUsername = proxy.Username;
                    request.ProxyPassword = proxy.Password;
                    request.ProxyType = proxy.Type;
                }
            }

            // Đăng ký tài khoản TikTok qua API với proxy
            var registrationResult = await _tiktokApiService.RegisterWithApiAsync(request);

            if (registrationResult.Success && registrationResult.Account != null)
            {
                // Nếu cần xác thực email, tự động lấy mã từ inboxes.com
                if (!registrationResult.Account.IsVerified)
                {
                    var verificationCode = await _inboxesEmailService.GetVerificationCodeAsync(
                        request.Email, 
                        timeoutSeconds: 120
                    );

                    if (!string.IsNullOrEmpty(verificationCode))
                    {
                        // Tự động xác thực email
                        var verifyResult = await _tiktokApiService.VerifyEmailWithApiAsync(
                            request.Email, 
                            verificationCode
                        );

                        if (verifyResult)
                        {
                            registrationResult.Account.IsVerified = true;
                            registrationResult.Message = "Đăng ký và xác thực email thành công";
                        }
                        else
                        {
                            registrationResult.AdditionalData = new Dictionary<string, object>
                            {
                                { "VerificationCode", verificationCode },
                                { "NeedsManualVerification", true }
                            };
                        }
                    }
                    else
                    {
                        registrationResult.AdditionalData = new Dictionary<string, object>
                        {
                            { "Email", request.Email },
                            { "NeedsManualVerification", true },
                            { "Message", "Không thể lấy mã xác nhận tự động, vui lòng kiểm tra email thủ công" }
                        };
                    }
                }

                return registrationResult;
            }

            return registrationResult;
        }
        catch (Exception ex)
        {
            return new RegistrationResponse
            {
                Success = false,
                Message = $"Lỗi trong quá trình đăng ký: {ex.Message}",
                ErrorCode = "EXCEPTION"
            };
        }
    }

    public async Task<bool> VerifyEmailAsync(string email, string verificationCode)
    {
        try
        {
            // Nếu không có mã xác nhận, tự động lấy từ inboxes.com
            if (string.IsNullOrEmpty(verificationCode))
            {
                var code = await _inboxesEmailService.GetVerificationCodeAsync(email, timeoutSeconds: 60);
                if (string.IsNullOrEmpty(code))
                {
                    return false;
                }
                verificationCode = code;
            }

            return await _tiktokApiService.VerifyEmailWithApiAsync(email, verificationCode);
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> VerifyPhoneAsync(string phoneNumber, string verificationCode)
    {
        // TikTok API có thể hỗ trợ verify phone tương tự
        return await VerifyEmailAsync(phoneNumber, verificationCode);
    }
}

