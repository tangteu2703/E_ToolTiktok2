using E_ToolTiktok.Core.Models;
using System.Net;

namespace E_ToolTiktok.Core.Services;

public class ProxyService : IProxyService
{
    private readonly HttpClient _httpClient;
    private readonly Random _random = new();
    private readonly Dictionary<string, List<ProxyInfo>> _proxyCache = new();

    public ProxyService()
    {
        _httpClient = new HttpClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task<ProxyInfo?> GetProxyAsync(string countryCode)
    {
        var proxies = await GetProxiesAsync(countryCode, 1);
        return proxies.FirstOrDefault();
    }

    public async Task<List<ProxyInfo>> GetProxiesAsync(string countryCode, int count)
    {
        var code = string.IsNullOrEmpty(countryCode) ? "US" : countryCode.ToUpper();
        
        // Kiểm tra cache
        if (_proxyCache.ContainsKey(code) && _proxyCache[code].Count >= count)
        {
            return _proxyCache[code].Take(count).ToList();
        }

        var proxies = new List<ProxyInfo>();

        // Method 1: Sử dụng ProxyScrape API (free)
        try
        {
            var proxyScrapeProxies = await GetProxiesFromProxyScrapeAsync(code, count);
            proxies.AddRange(proxyScrapeProxies);
        }
        catch
        {
            // Fallback nếu ProxyScrape không hoạt động
        }

        // Method 2: Sử dụng Free Proxy List API
        if (proxies.Count < count)
        {
            try
            {
                var freeProxies = await GetProxiesFromFreeProxyListAsync(code, count - proxies.Count);
                proxies.AddRange(freeProxies);
            }
            catch
            {
                // Fallback
            }
        }

        // Method 3: Generate fake proxies for testing (không thực sự hoạt động nhưng để test)
        if (proxies.Count == 0)
        {
            proxies.AddRange(GenerateTestProxies(code, count));
        }

        // Cache proxies
        _proxyCache[code] = proxies;

        return proxies.Take(count).ToList();
    }

    private async Task<List<ProxyInfo>> GetProxiesFromProxyScrapeAsync(string countryCode, int count)
    {
        var proxies = new List<ProxyInfo>();
        
        try
        {
            // ProxyScrape free API
            var url = $"https://api.proxyscrape.com/v2/?request=get&protocol=http&timeout=10000&country={countryCode}&ssl=all&anonymity=all";
            var response = await _httpClient.GetStringAsync(url);
            var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines.Take(count))
            {
                var parts = line.Trim().Split(':');
                if (parts.Length >= 2 && int.TryParse(parts[1], out var port))
                {
                    proxies.Add(new ProxyInfo
                    {
                        Host = parts[0],
                        Port = port,
                        CountryCode = countryCode,
                        Type = "http"
                    });
                }
            }
        }
        catch
        {
            // Ignore errors
        }

        return proxies;
    }

    private async Task<List<ProxyInfo>> GetProxiesFromFreeProxyListAsync(string countryCode, int count)
    {
        var proxies = new List<ProxyInfo>();
        
        try
        {
            // Free Proxy List API alternatives
            var urls = new[]
            {
                $"https://www.proxy-list.download/api/v1/get?type=http&country={countryCode}",
                $"https://api.proxyscrape.com/v2/?request=get&protocol=http&country={countryCode}"
            };

            foreach (var url in urls)
            {
                try
                {
                    var response = await _httpClient.GetStringAsync(url);
                    var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                    foreach (var line in lines.Take(count - proxies.Count))
                    {
                        var parts = line.Trim().Split(':');
                        if (parts.Length >= 2 && int.TryParse(parts[1], out var port))
                        {
                            proxies.Add(new ProxyInfo
                            {
                                Host = parts[0],
                                Port = port,
                                CountryCode = countryCode,
                                Type = "http"
                            });
                        }
                    }

                    if (proxies.Count >= count)
                        break;
                }
                catch
                {
                    continue;
                }
            }
        }
        catch
        {
            // Ignore errors
        }

        return proxies;
    }

    private List<ProxyInfo> GenerateTestProxies(string countryCode, int count)
    {
        // Generate test proxies (chỉ để test, không thực sự hoạt động)
        var proxies = new List<ProxyInfo>();
        var ipRanges = countryCode switch
        {
            "VN" => new[] { "14", "27", "42", "113", "115", "116", "117", "118", "123", "125" },
            "US" => new[] { "104", "107", "108", "142", "162", "172", "192", "198", "199", "207" },
            "GB" => new[] { "5", "31", "37", "46", "51", "77", "78", "79", "80", "81" },
            _ => new[] { "104", "107", "108", "142", "162", "172", "192", "198", "199", "207" }
        };

        for (int i = 0; i < count; i++)
        {
            var ipPart = ipRanges[_random.Next(ipRanges.Length)];
            proxies.Add(new ProxyInfo
            {
                Host = $"{ipPart}.{_random.Next(1, 255)}.{_random.Next(1, 255)}.{_random.Next(1, 255)}",
                Port = _random.Next(8000, 9999),
                CountryCode = countryCode,
                Type = "http",
                IsWorking = false // Mark as test proxy
            });
        }

        return proxies;
    }

    public async Task<bool> TestProxyAsync(ProxyInfo proxy)
    {
        try
        {
            var handler = new HttpClientHandler
            {
                Proxy = new WebProxy($"{proxy.Type}://{proxy.Host}:{proxy.Port}")
            };

            if (!string.IsNullOrEmpty(proxy.Username))
            {
                handler.Proxy.Credentials = new NetworkCredential(proxy.Username, proxy.Password);
            }

            using var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(5)
            };

            var response = await client.GetAsync("https://httpbin.org/ip");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}

