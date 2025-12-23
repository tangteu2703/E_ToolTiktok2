namespace E_ToolTiktok.Core.Models;

public class Country
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameVi { get; set; } = string.Empty;
    public string TimeZone { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
}

public static class Countries
{
    public static readonly List<Country> All = new()
    {
        new Country { Code = "VN", Name = "Vietnam", NameVi = "Việt Nam", TimeZone = "Asia/Ho_Chi_Minh", Language = "vi", Region = "VN" },
        new Country { Code = "US", Name = "United States", NameVi = "Hoa Kỳ", TimeZone = "America/New_York", Language = "en", Region = "US" },
        new Country { Code = "GB", Name = "United Kingdom", NameVi = "Anh", TimeZone = "Europe/London", Language = "en", Region = "GB" },
        new Country { Code = "CA", Name = "Canada", NameVi = "Canada", TimeZone = "America/Toronto", Language = "en", Region = "CA" },
        new Country { Code = "AU", Name = "Australia", NameVi = "Úc", TimeZone = "Australia/Sydney", Language = "en", Region = "AU" },
        new Country { Code = "JP", Name = "Japan", NameVi = "Nhật Bản", TimeZone = "Asia/Tokyo", Language = "ja", Region = "JP" },
        new Country { Code = "KR", Name = "South Korea", NameVi = "Hàn Quốc", TimeZone = "Asia/Seoul", Language = "ko", Region = "KR" },
        new Country { Code = "TH", Name = "Thailand", NameVi = "Thái Lan", TimeZone = "Asia/Bangkok", Language = "th", Region = "TH" },
        new Country { Code = "SG", Name = "Singapore", NameVi = "Singapore", TimeZone = "Asia/Singapore", Language = "en", Region = "SG" },
        new Country { Code = "MY", Name = "Malaysia", NameVi = "Malaysia", TimeZone = "Asia/Kuala_Lumpur", Language = "en", Region = "MY" },
        new Country { Code = "ID", Name = "Indonesia", NameVi = "Indonesia", TimeZone = "Asia/Jakarta", Language = "id", Region = "ID" },
        new Country { Code = "PH", Name = "Philippines", NameVi = "Philippines", TimeZone = "Asia/Manila", Language = "en", Region = "PH" },
        new Country { Code = "DE", Name = "Germany", NameVi = "Đức", TimeZone = "Europe/Berlin", Language = "de", Region = "DE" },
        new Country { Code = "FR", Name = "France", NameVi = "Pháp", TimeZone = "Europe/Paris", Language = "fr", Region = "FR" },
        new Country { Code = "IT", Name = "Italy", NameVi = "Ý", TimeZone = "Europe/Rome", Language = "it", Region = "IT" },
        new Country { Code = "ES", Name = "Spain", NameVi = "Tây Ban Nha", TimeZone = "Europe/Madrid", Language = "es", Region = "ES" },
        new Country { Code = "BR", Name = "Brazil", NameVi = "Brazil", TimeZone = "America/Sao_Paulo", Language = "pt", Region = "BR" },
        new Country { Code = "MX", Name = "Mexico", NameVi = "Mexico", TimeZone = "America/Mexico_City", Language = "es", Region = "MX" },
        new Country { Code = "IN", Name = "India", NameVi = "Ấn Độ", TimeZone = "Asia/Kolkata", Language = "en", Region = "IN" },
        new Country { Code = "CN", Name = "China", NameVi = "Trung Quốc", TimeZone = "Asia/Shanghai", Language = "zh", Region = "CN" }
    };

    public static Country? GetByCode(string code)
    {
        return All.FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
    }
}

