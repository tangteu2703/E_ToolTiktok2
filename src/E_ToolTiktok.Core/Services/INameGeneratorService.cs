using E_ToolTiktok.Core.Models;

namespace E_ToolTiktok.Core.Services;

public interface INameGeneratorService
{
    Task<string> GenerateFirstNameAsync(string countryCode);
    Task<string> GenerateLastNameAsync(string countryCode);
    Task<string> GenerateFullNameAsync(string countryCode);
    Task<string> GenerateUsernameAsync(string countryCode, string? email = null);
}

