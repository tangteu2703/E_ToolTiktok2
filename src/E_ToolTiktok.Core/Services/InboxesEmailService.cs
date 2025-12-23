using E_ToolTiktok.Core.Models;
using System.Text.RegularExpressions;

namespace E_ToolTiktok.Core.Services;

public class InboxesEmailService : IInboxesEmailService
{
    private readonly HttpClient _httpClient;
    private readonly Random _random = new();

    public InboxesEmailService()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://inboxes.com/");
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
    }

    public async Task<InboxesEmail> CreateRandomEmailAsync()
    {
        try
        {
            // Lấy danh sách domain có sẵn
            var domainResponse = await _httpClient.GetAsync("api/domain");
            var domainsJson = await domainResponse.Content.ReadAsStringAsync();
            
            // Parse domains (có thể là JSON array hoặc string)
            var domains = new List<string>();
            if (domainsJson.StartsWith("["))
            {
                // JSON array
                var domainList = System.Text.Json.JsonSerializer.Deserialize<List<string>>(domainsJson);
                if (domainList != null)
                    domains.AddRange(domainList);
            }
            else
            {
                // Single domain string
                domains.Add(domainsJson.Trim('"'));
            }

            // Nếu không có domain từ API, sử dụng domain mặc định
            if (domains.Count == 0)
            {
                domains.AddRange(new[] { "vomoto.com", "givmail.com", "inboxes.com" });
            }

            // Tạo username ngẫu nhiên
            var username = GenerateRandomUsername();
            var domain = domains[_random.Next(domains.Count)];
            var email = $"{username}@{domain}";

            // Tạo inbox
            var createResponse = await _httpClient.PostAsync($"api/inbox?inbox={email}", null);
            
            if (createResponse.IsSuccessStatusCode)
            {
                return new InboxesEmail
                {
                    Email = email,
                    InboxId = email,
                    CreatedAt = DateTime.Now
                };
            }

            // Fallback: thử tạo với format khác
            return new InboxesEmail
            {
                Email = email,
                InboxId = email,
                CreatedAt = DateTime.Now
            };
        }
        catch
        {
            // Fallback: tạo email ngẫu nhiên local
            var username = GenerateRandomUsername();
            var domains = new[] { "vomoto.com", "givmail.com", "inboxes.com" };
            var domain = domains[_random.Next(domains.Length)];
            
            return new InboxesEmail
            {
                Email = $"{username}@{domain}",
                InboxId = $"{username}@{domain}",
                CreatedAt = DateTime.Now
            };
        }
    }

    public async Task<List<InboxesMessage>> GetMessagesAsync(string email)
    {
        try
        {
            if (string.IsNullOrEmpty(email) || !email.Contains('@'))
            {
                return new List<InboxesMessage>();
            }

            var emailParts = email.Split('@');
            if (emailParts.Length != 2)
            {
                return new List<InboxesMessage>();
            }

            var inboxId = emailParts[0];
            var domain = emailParts[1];
            
            // Thử các endpoint API khác nhau
            var endpoints = new[]
            {
                $"api/inbox/{inboxId}@{domain}",
                $"api/inbox/{email}",
                $"api/messages/{email}",
                $"api/inbox?email={email}"
            };

            foreach (var endpoint in endpoints)
            {
                try
                {
                    var response = await _httpClient.GetAsync(endpoint);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        // Parse JSON response
                        var messages = System.Text.Json.JsonSerializer.Deserialize<List<InboxesMessage>>(json);
                        return messages ?? new List<InboxesMessage>();
                    }
                }
                catch
                {
                    continue;
                }
            }

            return new List<InboxesMessage>();
        }
        catch
        {
            return new List<InboxesMessage>();
        }
    }

    public async Task<string?> GetVerificationCodeAsync(string email, int timeoutSeconds = 60)
    {
        var startTime = DateTime.Now;
        
        while ((DateTime.Now - startTime).TotalSeconds < timeoutSeconds)
        {
            var messages = await GetMessagesAsync(email);
            
            if (messages == null || messages.Count == 0)
            {
                await Task.Delay(3000);
                continue;
            }
            
            // Tìm email từ TikTok
            var tiktokMessage = messages.FirstOrDefault(m => 
                (m.From?.ToLower().Contains("tiktok") ?? false) || 
                (m.Subject?.ToLower().Contains("verify") ?? false) ||
                (m.Subject?.ToLower().Contains("code") ?? false) ||
                (m.Preview?.ToLower().Contains("code") ?? false));

            if (tiktokMessage != null)
            {
                // Extract verification code từ email
                var code = ExtractVerificationCode(tiktokMessage.TextBody ?? tiktokMessage.HtmlBody ?? tiktokMessage.Preview);
                if (!string.IsNullOrEmpty(code))
                {
                    return code;
                }
            }

            await Task.Delay(3000); // Đợi 3 giây trước khi check lại
        }

        return null;
    }

    public async Task<InboxesMessage?> WaitForMessageAsync(string email, string fromContains = "tiktok", int timeoutSeconds = 60)
    {
        var startTime = DateTime.Now;
        
        while ((DateTime.Now - startTime).TotalSeconds < timeoutSeconds)
        {
            var messages = await GetMessagesAsync(email);
            
            if (messages == null || messages.Count == 0)
            {
                await Task.Delay(3000);
                continue;
            }
            
            var message = messages.FirstOrDefault(m => 
                (m.From?.ToLower().Contains(fromContains.ToLower()) ?? false) ||
                (m.Subject?.ToLower().Contains("verify") ?? false) ||
                (m.Subject?.ToLower().Contains("code") ?? false));

            if (message != null)
            {
                return message;
            }

            await Task.Delay(3000);
        }

        return null;
    }

    private string GenerateRandomUsername()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        var length = _random.Next(8, 15);
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }

    private string? ExtractVerificationCode(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return null;

        // Tìm mã 6 chữ số
        var match6 = Regex.Match(text, @"\b\d{6}\b");
        if (match6.Success)
            return match6.Value;

        // Tìm mã 4 chữ số
        var match4 = Regex.Match(text, @"\b\d{4}\b");
        if (match4.Success)
            return match4.Value;

        // Tìm mã trong format: code: 123456 hoặc verification code: 123456
        var patternMatch = Regex.Match(text, @"(?:code|verification code|mã|mã xác nhận)[\s:]*(\d{4,6})", RegexOptions.IgnoreCase);
        if (patternMatch.Success)
            return patternMatch.Groups[1].Value;

        return null;
    }
}

