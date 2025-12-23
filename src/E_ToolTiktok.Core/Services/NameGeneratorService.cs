using E_ToolTiktok.Core.Models;

namespace E_ToolTiktok.Core.Services;

public class NameGeneratorService : INameGeneratorService
{
    private readonly Random _random = new();
    private readonly Dictionary<string, List<string>> _firstNames = new();
    private readonly Dictionary<string, List<string>> _lastNames = new();

    public NameGeneratorService()
    {
        InitializeNames();
    }

    private void InitializeNames()
    {
        // Vietnamese names
        _firstNames["VN"] = new List<string>
        {
            "An", "Bình", "Chi", "Dũng", "Hạnh", "Hùng", "Lan", "Minh", "Nam", "Phương",
            "Quang", "Thảo", "Tuấn", "Vy", "Đức", "Giang", "Hoa", "Khang", "Linh", "My",
            "Ngọc", "Oanh", "Phong", "Quyên", "Sơn", "Thành", "Uyên", "Vân", "Xuân", "Yến"
        };

        _lastNames["VN"] = new List<string>
        {
            "Nguyễn", "Trần", "Lê", "Phạm", "Hoàng", "Huỳnh", "Phan", "Vũ", "Võ", "Đặng",
            "Bùi", "Đỗ", "Hồ", "Ngô", "Dương", "Đinh", "Đào", "Đoàn", "Lý", "Lương"
        };

        // US/English names
        _firstNames["US"] = new List<string>
        {
            "James", "Mary", "John", "Patricia", "Robert", "Jennifer", "Michael", "Linda",
            "William", "Elizabeth", "David", "Barbara", "Richard", "Susan", "Joseph", "Jessica",
            "Thomas", "Sarah", "Christopher", "Karen", "Daniel", "Nancy", "Matthew", "Lisa",
            "Anthony", "Betty", "Mark", "Margaret", "Donald", "Sandra", "Steven", "Ashley"
        };

        _lastNames["US"] = new List<string>
        {
            "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis",
            "Rodriguez", "Martinez", "Hernandez", "Lopez", "Wilson", "Anderson", "Thomas", "Taylor",
            "Moore", "Jackson", "Martin", "Lee", "Thompson", "White", "Harris", "Sanchez"
        };

        // UK names (similar to US but some variations)
        _firstNames["GB"] = _firstNames["US"];
        _lastNames["GB"] = new List<string>
        {
            "Smith", "Jones", "Taylor", "Williams", "Brown", "Davies", "Evans", "Wilson",
            "Thomas", "Roberts", "Johnson", "Lewis", "Walker", "Robinson", "Wood", "Thompson",
            "White", "Watson", "Jackson", "Wright", "Green", "Harris", "Cooper", "King"
        };

        // Japanese names
        _firstNames["JP"] = new List<string>
        {
            "Hiroshi", "Yuki", "Takeshi", "Sakura", "Kenji", "Aiko", "Ryota", "Mei",
            "Satoshi", "Yui", "Daiki", "Rina", "Kenta", "Emi", "Shota", "Mika",
            "Yuto", "Akari", "Riku", "Hana", "Ren", "Mio", "Haruto", "Yuna"
        };

        _lastNames["JP"] = new List<string>
        {
            "Tanaka", "Sato", "Suzuki", "Takahashi", "Watanabe", "Ito", "Yamamoto", "Nakamura",
            "Kobayashi", "Kato", "Yoshida", "Yamada", "Sasaki", "Yamaguchi", "Matsumoto", "Inoue",
            "Kimura", "Hayashi", "Shimizu", "Yamazaki", "Mori", "Abe", "Ikeda", "Hashimoto"
        };

        // Korean names
        _firstNames["KR"] = new List<string>
        {
            "Min-jun", "Soo-jin", "Ji-hoon", "Seo-yeon", "Hyun-woo", "Ji-woo", "Seung-ho", "Min-seo",
            "Jun-seo", "Ye-eun", "Do-hyun", "Ha-eun", "Seo-jun", "Ji-eun", "Yoon-seo", "Ji-min"
        };

        _lastNames["KR"] = new List<string>
        {
            "Kim", "Lee", "Park", "Choi", "Jung", "Kang", "Cho", "Yoon",
            "Jang", "Lim", "Han", "Shin", "Seo", "Kwon", "Oh", "Song"
        };

        // Default to US names for other countries
        foreach (var country in Countries.All.Where(c => !_firstNames.ContainsKey(c.Code)))
        {
            _firstNames[country.Code] = _firstNames["US"];
            _lastNames[country.Code] = _lastNames["US"];
        }
    }

    public Task<string> GenerateFirstNameAsync(string countryCode)
    {
        var code = string.IsNullOrEmpty(countryCode) ? "US" : countryCode.ToUpper();
        if (!_firstNames.ContainsKey(code))
            code = "US";

        var names = _firstNames[code];
        var name = names[_random.Next(names.Count)];
        return Task.FromResult(name);
    }

    public Task<string> GenerateLastNameAsync(string countryCode)
    {
        var code = string.IsNullOrEmpty(countryCode) ? "US" : countryCode.ToUpper();
        if (!_lastNames.ContainsKey(code))
            code = "US";

        var names = _lastNames[code];
        var name = names[_random.Next(names.Count)];
        return Task.FromResult(name);
    }

    public async Task<string> GenerateFullNameAsync(string countryCode)
    {
        var firstName = await GenerateFirstNameAsync(countryCode);
        var lastName = await GenerateLastNameAsync(countryCode);
        
        // Format based on country
        return countryCode.ToUpper() switch
        {
            "VN" => $"{lastName} {firstName}",
            "JP" => $"{lastName} {firstName}",
            "KR" => $"{lastName} {firstName}",
            _ => $"{firstName} {lastName}"
        };
    }

    public async Task<string> GenerateUsernameAsync(string countryCode, string? email = null)
    {
        var firstName = await GenerateFirstNameAsync(countryCode);
        var lastName = await GenerateLastNameAsync(countryCode);
        var number = _random.Next(100, 9999);
        
        // Generate username based on country style
        return countryCode.ToUpper() switch
        {
            "VN" => $"{firstName.ToLower()}{lastName.ToLower()}{number}",
            "JP" => $"{lastName.ToLower()}{firstName.ToLower()}{number}",
            "KR" => $"{lastName.ToLower()}{firstName.ToLower()}{number}",
            _ => $"{firstName.ToLower()}{lastName.ToLower()}{number}"
        };
    }
}

