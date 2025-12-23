using E_ToolTiktok.Core.Models;
using E_ToolTiktok.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_ToolTiktok.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TiktokRegistrationController : ControllerBase
{
    private readonly ITiktokRegistrationService _registrationService;
    private readonly IInboxesEmailService _inboxesEmailService;
    private readonly ILogger<TiktokRegistrationController> _logger;

    public TiktokRegistrationController(
        ITiktokRegistrationService registrationService,
        IInboxesEmailService inboxesEmailService,
        ILogger<TiktokRegistrationController> logger)
    {
        _registrationService = registrationService;
        _inboxesEmailService = inboxesEmailService;
        _logger = logger;
    }

    /// <summary>
    /// Đăng ký tài khoản TikTok mới (tự động tạo email từ inboxes.com nếu không có)
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<RegistrationResponse>> Register([FromBody] RegistrationRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new RegistrationResponse
                {
                    Success = false,
                    Message = "Password là bắt buộc"
                });
            }

            // Nếu không có email, sẽ tự động tạo từ inboxes.com
            var emailUsed = request.Email ?? "Tự động tạo từ inboxes.com";
            _logger.LogInformation($"Bắt đầu đăng ký tài khoản với email: {emailUsed}");

            var result = await _registrationService.RegisterAccountAsync(request);

            if (result.Success)
            {
                _logger.LogInformation($"Đăng ký thành công cho email: {result.Account?.Email ?? emailUsed}");
                return Ok(result);
            }
            else
            {
                _logger.LogWarning($"Đăng ký thất bại cho email: {emailUsed}, Lỗi: {result.Message}, ErrorCode: {result.ErrorCode}");
                
                // Log thêm thông tin debug nếu có
                if (result.AdditionalData != null)
                {
                    foreach (var item in result.AdditionalData)
                    {
                        _logger.LogWarning($"  - {item.Key}: {item.Value}");
                    }
                }
                
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi đăng ký tài khoản: {ex.Message}");
            return StatusCode(500, new RegistrationResponse
            {
                Success = false,
                Message = $"Lỗi server: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Xác thực email
    /// </summary>
    [HttpPost("verify-email")]
    public async Task<ActionResult<bool>> VerifyEmail([FromBody] VerifyEmailRequest request)
    {
        try
        {
            var result = await _registrationService.VerifyEmailAsync(request.Email, request.VerificationCode);
            return Ok(new { Success = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi xác thực email: {ex.Message}");
            return StatusCode(500, new { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Xác thực số điện thoại
    /// </summary>
    [HttpPost("verify-phone")]
    public async Task<ActionResult<bool>> VerifyPhone([FromBody] VerifyPhoneRequest request)
    {
        try
        {
            var result = await _registrationService.VerifyPhoneAsync(request.PhoneNumber, request.VerificationCode);
            return Ok(new { Success = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi xác thực số điện thoại: {ex.Message}");
            return StatusCode(500, new { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy danh sách các quốc gia hỗ trợ
    /// </summary>
    [HttpGet("countries")]
    public ActionResult<List<Country>> GetCountries()
    {
        return Ok(Countries.All);
    }

    /// <summary>
    /// Tạo email ngẫu nhiên từ inboxes.com
    /// </summary>
    [HttpPost("create-email")]
    public async Task<ActionResult<InboxesEmail>> CreateEmail()
    {
        try
        {
            var email = await _inboxesEmailService.CreateRandomEmailAsync();
            return Ok(email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi tạo email: {ex.Message}");
            return StatusCode(500, new { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy danh sách email từ inbox
    /// </summary>
    [HttpGet("inbox/{email}/messages")]
    public async Task<ActionResult<List<InboxesMessage>>> GetMessages(string email)
    {
        try
        {
            if (string.IsNullOrEmpty(email) || !email.Contains('@'))
            {
                return BadRequest(new { Success = false, Message = "Email không hợp lệ" });
            }

            // Decode URL encoding
            email = Uri.UnescapeDataString(email);
            
            var messages = await _inboxesEmailService.GetMessagesAsync(email);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi lấy messages: {ex.Message}");
            return StatusCode(500, new { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy mã xác nhận từ email (tự động tìm trong inbox)
    /// </summary>
    [HttpGet("inbox/{email}/verification-code")]
    public async Task<ActionResult<object>> GetVerificationCode(string email, [FromQuery] int timeoutSeconds = 60)
    {
        try
        {
            if (string.IsNullOrEmpty(email) || !email.Contains('@'))
            {
                return BadRequest(new { Success = false, Message = "Email không hợp lệ" });
            }

            if (timeoutSeconds < 10 || timeoutSeconds > 300)
            {
                timeoutSeconds = 60; // Default to 60 seconds if invalid
            }

            // Decode URL encoding
            email = Uri.UnescapeDataString(email);
            
            var code = await _inboxesEmailService.GetVerificationCodeAsync(email, timeoutSeconds);
            
            if (string.IsNullOrEmpty(code))
            {
                return NotFound(new { Success = false, Message = "Không tìm thấy mã xác nhận trong thời gian chờ" });
            }

            return Ok(new { Success = true, VerificationCode = code });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi lấy mã xác nhận: {ex.Message}");
            return StatusCode(500, new { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Lưu tài khoản vào file txt
    /// </summary>
    [HttpPost("save-account")]
    public async Task<ActionResult> SaveAccount([FromBody] SaveAccountRequest request)
    {
        try
        {
            var accountsDir = Path.Combine(Directory.GetCurrentDirectory(), "Accounts");
            if (!Directory.Exists(accountsDir))
            {
                Directory.CreateDirectory(accountsDir);
            }

            var fileName = $"accounts_{DateTime.Now:yyyyMMdd}.txt";
            var filePath = Path.Combine(accountsDir, fileName);

            var accountLine = $"Email: {request.Email} | Username: {request.Username} | Password: {request.Password} | Verified: {request.IsVerified} | Created: {request.CreatedAt}\n";
            
            await System.IO.File.AppendAllTextAsync(filePath, accountLine);

            _logger.LogInformation($"Đã lưu tài khoản vào file: {filePath}");

            return Ok(new { Success = true, Message = "Đã lưu tài khoản thành công", FilePath = filePath });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi lưu tài khoản: {ex.Message}");
            return StatusCode(500, new { Success = false, Message = $"Lỗi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Lấy danh sách tài khoản đã lưu
    /// </summary>
    [HttpGet("saved-accounts")]
    public ActionResult<object> GetSavedAccounts()
    {
        try
        {
            var accountsDir = Path.Combine(Directory.GetCurrentDirectory(), "Accounts");
            if (!Directory.Exists(accountsDir))
            {
                return Ok(new { Accounts = new List<string>(), Total = 0 });
            }

            var files = Directory.GetFiles(accountsDir, "accounts_*.txt");
            var allAccounts = new List<string>();

            foreach (var file in files)
            {
                var lines = System.IO.File.ReadAllLines(file);
                allAccounts.AddRange(lines);
            }

            return Ok(new { Accounts = allAccounts, Total = allAccounts.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi đọc danh sách tài khoản: {ex.Message}");
            return StatusCode(500, new { Success = false, Message = $"Lỗi: {ex.Message}" });
        }
    }

    /// <summary>
    /// Kiểm tra trạng thái API
    /// </summary>
    [HttpGet("health")]
    public ActionResult<object> Health()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0"
        });
    }
}

public class SaveAccountRequest
{
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
}

public class VerifyEmailRequest
{
    public string Email { get; set; } = string.Empty;
    public string VerificationCode { get; set; } = string.Empty;
}

public class VerifyPhoneRequest
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string VerificationCode { get; set; } = string.Empty;
}

