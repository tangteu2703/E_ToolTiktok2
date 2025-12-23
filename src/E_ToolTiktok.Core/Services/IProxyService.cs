using E_ToolTiktok.Core.Models;

namespace E_ToolTiktok.Core.Services;

public interface IProxyService
{
    Task<ProxyInfo?> GetProxyAsync(string countryCode);
    Task<List<ProxyInfo>> GetProxiesAsync(string countryCode, int count);
    Task<bool> TestProxyAsync(ProxyInfo proxy);
}

public class ProxyInfo
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public string Type { get; set; } = "http"; // http, https, socks5
    public bool IsWorking { get; set; } = true;
}

