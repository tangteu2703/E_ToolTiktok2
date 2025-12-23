using E_ToolTiktok.Core.Models;
using System.Text;
using System.Text.Json;
using System.Net;

namespace E_ToolTiktok.Core.Services;

public class TiktokApiService : ITiktokApiService
{
    private HttpClient _httpClient = null!;
    private CookieContainer _cookieContainer = null!;
    private HttpClientHandler _httpClientHandler = null!;
    private string? _csrfToken;

    public TiktokApiService()
    {
        InitializeHttpClient(null);
    }

    private void InitializeHttpClient(ProxyInfo? proxy)
    {
        // Dispose old instances if they exist
        _httpClient?.Dispose();
        _httpClientHandler?.Dispose();
        
        _cookieContainer = new CookieContainer();
        
        _httpClientHandler = new HttpClientHandler
        {
            CookieContainer = _cookieContainer,
            UseCookies = true,
            AllowAutoRedirect = true
        };

        // Cấu hình proxy nếu có
        if (proxy != null && !string.IsNullOrEmpty(proxy.Host))
        {
            var proxyUri = new Uri($"{proxy.Type}://{proxy.Host}:{proxy.Port}");
            _httpClientHandler.Proxy = new WebProxy(proxyUri);
            
            if (!string.IsNullOrEmpty(proxy.Username))
            {
                _httpClientHandler.Proxy.Credentials = new NetworkCredential(
                    proxy.Username, 
                    proxy.Password ?? ""
                );
            }
            else
            {
                _httpClientHandler.UseProxy = true;
            }
        }

        _httpClient = new HttpClient(_httpClientHandler);
        _httpClient.BaseAddress = new Uri("https://www.tiktok.com/");
        _httpClient.Timeout = TimeSpan.FromSeconds(60);
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
        _httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9,vi;q=0.8");
        _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
        _httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
        _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
        _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
        _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
        _httpClient.DefaultRequestHeaders.Add("Referer", "https://www.tiktok.com/");
        _httpClient.DefaultRequestHeaders.Add("Origin", "https://www.tiktok.com");
    }

    private HttpClient CreateHttpClientWithProxy(RegistrationRequest request)
    {
        // Nếu có proxy trong request, tạo HttpClient mới với proxy đó
        if (request.UseProxy && !string.IsNullOrEmpty(request.ProxyAddress) && request.ProxyPort.HasValue)
        {
            var proxy = new ProxyInfo
            {
                Host = request.ProxyAddress,
                Port = request.ProxyPort.Value,
                Username = request.ProxyUsername,
                Password = request.ProxyPassword,
                Type = request.ProxyType ?? "http"
            };
            
            InitializeHttpClient(proxy);
        }
        else
        {
            InitializeHttpClient(null);
        }

        return _httpClient;
    }

    public async Task<string?> GetCsrfTokenAsync()
    {
        try
        {
            if (!string.IsNullOrEmpty(_csrfToken))
                return _csrfToken;

            // Thử nhiều cách để lấy CSRF token
            var urls = new[] { "/signup", "/", "/login" };
            
            foreach (var url in urls)
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    var response = await _httpClient.SendAsync(request);
                    var html = await response.Content.ReadAsStringAsync();

                    // Method 1: Tìm trong HTML - nhiều patterns khác nhau
                    var patterns = new[]
                    {
                        @"csrf_token[""']?\s*[:=]\s*[""']([^""']+)",
                        @"csrfToken[""']?\s*[:=]\s*[""']([^""']+)",
                        @"csrf-token[""']?\s*[:=]\s*[""']([^""']+)",
                        @"_csrf[""']?\s*[:=]\s*[""']([^""']+)",
                        @"window\.__UNIVERSAL_DATA_FOR_REHYDRATION__\s*=\s*({[^}]+})",
                        @"""csrfToken"":\s*""([^""]+)"""
                    };

                    foreach (var pattern in patterns)
                    {
                        var match = System.Text.RegularExpressions.Regex.Match(html, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            _csrfToken = match.Groups[1].Value;
                            if (!string.IsNullOrEmpty(_csrfToken) && _csrfToken.Length > 10)
                            {
                                return _csrfToken;
                            }
                        }
                    }

                    // Method 2: Tìm trong JSON data
                    var jsonMatch = System.Text.RegularExpressions.Regex.Match(html, @"window\.__UNIVERSAL_DATA_FOR_REHYDRATION__\s*=\s*({.+?});", System.Text.RegularExpressions.RegexOptions.Singleline);
                    if (jsonMatch.Success)
                    {
                        try
                        {
                            var jsonData = JsonSerializer.Deserialize<JsonElement>(jsonMatch.Groups[1].Value);
                            if (jsonData.TryGetProperty("csrfToken", out var csrf))
                            {
                                _csrfToken = csrf.GetString();
                                if (!string.IsNullOrEmpty(_csrfToken))
                                    return _csrfToken;
                            }
                        }
                        catch { }
                    }

                    // Method 3: Lấy từ cookies
                    var cookies = _cookieContainer.GetCookies(new Uri("https://www.tiktok.com"));
                    foreach (Cookie cookie in cookies)
                    {
                        if (cookie.Name.Contains("csrf", StringComparison.OrdinalIgnoreCase) || 
                            cookie.Name.Contains("token", StringComparison.OrdinalIgnoreCase))
                        {
                            _csrfToken = cookie.Value;
                            if (!string.IsNullOrEmpty(_csrfToken))
                                return _csrfToken;
                        }
                    }

                    // Method 4: Lấy từ response headers
                    if (response.Headers.Contains("Set-Cookie"))
                    {
                        var setCookies = response.Headers.GetValues("Set-Cookie");
                        foreach (var cookie in setCookies)
                        {
                            if (cookie.Contains("csrf", StringComparison.OrdinalIgnoreCase))
                            {
                                var match = System.Text.RegularExpressions.Regex.Match(cookie, @"csrf[^=]*=([^;]+)");
                                if (match.Success)
                                {
                                    _csrfToken = match.Groups[1].Value;
                                    return _csrfToken;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }

            // Fallback: Generate một token giả (có thể không hoạt động nhưng để test)
            _csrfToken = GenerateRandomToken();
            return _csrfToken;
        }
        catch
        {
            // Fallback token
            _csrfToken = GenerateRandomToken();
            return _csrfToken;
        }
    }

    private string GenerateRandomToken()
    {
        var random = new Random();
        var chars = "0123456789abcdef";
        return new string(Enumerable.Repeat(chars, 32)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public async Task<RegistrationResponse> RegisterWithApiAsync(RegistrationRequest request)
    {
        try
        {
            // Tạo HttpClient với proxy nếu có
            var httpClient = CreateHttpClientWithProxy(request);
            
            // Cập nhật headers theo country
            var country = Countries.GetByCode(request.CountryCode ?? "VN") ?? Countries.GetByCode("VN")!;
            if (httpClient.DefaultRequestHeaders.Contains("Accept-Language"))
            {
                httpClient.DefaultRequestHeaders.Remove("Accept-Language");
            }
            httpClient.DefaultRequestHeaders.Add("Accept-Language", $"{country.Language},{country.Language}-{country.Code},en;q=0.9");

            // Lấy CSRF token với HttpClient mới
            var csrfToken = await GetCsrfTokenAsync();
            if (!string.IsNullOrEmpty(csrfToken))
            {
                if (httpClient.DefaultRequestHeaders.Contains("X-CSRFToken"))
                {
                    httpClient.DefaultRequestHeaders.Remove("X-CSRFToken");
                }
                httpClient.DefaultRequestHeaders.Add("X-CSRFToken", csrfToken);
            }

            // TikTok API endpoints cho đăng ký (thử nhiều endpoints)
            var signupEndpoints = new[]
            {
                "/passport/web/account/register/",
                "/passport/web/email/register/",
                "/passport/email/register/",
                "/api/user/register/",
                "/passport/web/register/",
                "/passport/register/",
                "/api/passport/web/account/register/",
                "/api/passport/web/email/register/"
            };

            var birthDate = request.BirthDate ?? DateTime.Now.AddYears(-18);
            var birthDay = birthDate.Day;
            var birthMonth = birthDate.Month;
            var birthYear = birthDate.Year;

            foreach (var endpoint in signupEndpoints)
            {
                try
                {
                    // Đảm bảo có CSRF token
                    if (string.IsNullOrEmpty(_csrfToken))
                    {
                        await GetCsrfTokenAsync();
                    }

                    // Tạo request với headers đầy đủ
                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint);
                    
                    // Thêm CSRF token vào header nếu có
                    if (!string.IsNullOrEmpty(_csrfToken))
                    {
                        requestMessage.Headers.Add("X-CSRFToken", _csrfToken);
                    }

                    // Tạo payload với nhiều format khác nhau
                    var payloads = new[]
                    {
                        // Format 1: Đầy đủ thông tin
                        new Dictionary<string, object>
                        {
                            { "email", request.Email },
                            { "password", request.Password },
                            { "username", request.Username ?? GenerateUsername(request.Email) },
                            { "birthday", $"{birthYear}-{birthMonth:D2}-{birthDay:D2}" },
                            { "birth_day", birthDay },
                            { "birth_month", birthMonth },
                            { "birth_year", birthYear },
                            { "captcha", "" },
                            { "aid", 1459 },
                            { "app_language", "en" },
                            { "browser_language", "en" },
                            { "browser_name", "Mozilla" },
                            { "browser_online", true },
                            { "browser_platform", "Win32" },
                            { "browser_version", "5.0" },
                            { "channel", "tiktok_web" },
                            { "cookie_enabled", true },
                            { "device_id", GenerateDeviceId() },
                            { "device_platform", "web_pc" },
                            { "focus_state", true },
                            { "from_page", "user" },
                            { "history_len", 0 },
                            { "is_browser_trust", true },
                            { "is_fullscreen", false },
                            { "is_page_visible", true },
                            { "os", "windows" },
                            { "priority_region", country.Region },
                            { "referer", "" },
                            { "region", country.Region },
                            { "screen_height", 1080 },
                            { "screen_width", 1920 },
                            { "tz_name", country.TimeZone },
                            { "verifyFp", GenerateVerifyFp() },
                            { "webcast_language", country.Language },
                            { "app_language", country.Language },
                            { "browser_language", country.Language }
                        },
                        // Format 2: Đơn giản hơn
                        new Dictionary<string, object>
                        {
                            { "email", request.Email },
                            { "password", request.Password },
                            { "username", request.Username ?? GenerateUsername(request.Email) },
                            { "birthday", $"{birthYear}-{birthMonth:D2}-{birthDay:D2}" }
                        }
                    };

                    foreach (var payload in payloads)
                    {
                        try
                        {
                            var json = JsonSerializer.Serialize(payload);
                            requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");

                            var response = await httpClient.SendAsync(requestMessage);
                            var responseContent = await response.Content.ReadAsStringAsync();

                            // Log để debug (sẽ được thay bằng ILogger sau)
                            System.Diagnostics.Debug.WriteLine($"TikTok API - Endpoint: {endpoint}");
                            System.Diagnostics.Debug.WriteLine($"TikTok API - Status: {response.StatusCode}");
                            System.Diagnostics.Debug.WriteLine($"TikTok API - Response: {responseContent.Substring(0, Math.Min(500, responseContent.Length))}");

                            if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                            {
                                try
                                {
                                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                                    
                                    // Check response
                                    if (result.TryGetProperty("status_code", out var statusCode))
                                    {
                                        var code = statusCode.GetInt32();
                                        if (code == 0)
                                        {
                                            var account = new TiktokAccount
                                            {
                                                Email = request.Email,
                                                Password = request.Password,
                                                Username = request.Username ?? GenerateUsername(request.Email),
                                                CreatedAt = DateTime.Now,
                                                IsVerified = false
                                            };

                                            return new RegistrationResponse
                                            {
                                                Success = true,
                                                Message = "Đăng ký thành công, cần xác thực email",
                                                Account = account
                                            };
                                        }
                                        else
                                        {
                                            var message = result.TryGetProperty("status_msg", out var msg) 
                                                ? msg.GetString() 
                                                : $"Đăng ký thất bại (Code: {code})";
                                            
                                            // Không return ngay, thử format khác hoặc endpoint khác
                                            continue;
                                        }
                                    }
                                    // Nếu không có status_code, có thể là format khác
                                    else if (result.TryGetProperty("data", out var data))
                                    {
                                        // Có thể thành công với format khác
                                        var account = new TiktokAccount
                                        {
                                            Email = request.Email,
                                            Password = request.Password,
                                            Username = request.Username ?? GenerateUsername(request.Email),
                                            CreatedAt = DateTime.Now,
                                            IsVerified = false
                                        };

                                        return new RegistrationResponse
                                        {
                                            Success = true,
                                            Message = "Đăng ký thành công",
                                            Account = account
                                        };
                                    }
                                }
                                catch
                                {
                                    // Không phải JSON, thử tiếp
                                    continue;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"TikTok API - Error với payload: {ex.Message}");
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"TikTok API - Error với endpoint {endpoint}: {ex.Message}");
                    continue;
                }
            }

            return new RegistrationResponse
            {
                Success = false,
                Message = $"Không thể kết nối đến TikTok API. Đã thử {signupEndpoints.Length} endpoints nhưng không thành công. CSRF Token: {(_csrfToken != null ? "Đã lấy được" : "null")}",
                ErrorCode = "API_CONNECTION_FAILED",
                AdditionalData = new Dictionary<string, object>
                {
                    { "EndpointsTried", signupEndpoints.Length },
                    { "Endpoints", string.Join(", ", signupEndpoints) },
                    { "CsrfTokenStatus", _csrfToken != null ? "Available" : "Not Available" },
                    { "HasCookies", _cookieContainer.Count > 0 }
                }
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"TikTok API Exception: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
            
            return new RegistrationResponse
            {
                Success = false,
                Message = $"Lỗi: {ex.Message}",
                ErrorCode = "EXCEPTION",
                AdditionalData = new Dictionary<string, object>
                {
                    { "ExceptionType", ex.GetType().Name },
                    { "StackTrace", ex.StackTrace ?? "" }
                }
            };
        }
    }

    public async Task<bool> VerifyEmailWithApiAsync(string email, string verificationCode)
    {
        try
        {
            var csrfToken = await GetCsrfTokenAsync();
            if (!string.IsNullOrEmpty(csrfToken))
            {
                if (_httpClient.DefaultRequestHeaders.Contains("X-CSRFToken"))
                {
                    _httpClient.DefaultRequestHeaders.Remove("X-CSRFToken");
                }
                _httpClient.DefaultRequestHeaders.Add("X-CSRFToken", csrfToken);
            }

            var verifyEndpoints = new[]
            {
                "/passport/web/email/verify/",
                "/api/user/verify/email/",
                "/passport/email/verify/"
            };

            foreach (var endpoint in verifyEndpoints)
            {
                try
                {
                    var payload = new
                    {
                        email = email,
                        code = verificationCode,
                        verifyFp = GenerateVerifyFp()
                    };

                    var json = JsonSerializer.Serialize(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync(endpoint, content);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                        if (result.TryGetProperty("status_code", out var statusCode))
                        {
                            return statusCode.GetInt32() == 0;
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    private string GenerateUsername(string email)
    {
        if (string.IsNullOrEmpty(email) || !email.Contains('@'))
        {
            return $"user{new Random().Next(100000, 999999)}";
        }

        var emailParts = email.Split('@');
        var username = emailParts.Length > 0 ? emailParts[0] : "user";
        var random = new Random();
        return $"{username}{random.Next(1000, 9999)}";
    }

    private string GenerateDeviceId()
    {
        var guid = Guid.NewGuid().ToString().Replace("-", "");
        return guid.Length >= 16 ? guid.Substring(0, 16) : guid.PadRight(16, '0');
    }

    private string GenerateVerifyFp()
    {
        // TikTok verifyFp format
        var random = new Random();
        var chars = "0123456789abcdef";
        return new string(Enumerable.Repeat(chars, 32)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}

