using System.Text.Json;
using AdminSettings.Persistence.Entities;

namespace AdminSettings.Services;

public class UserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<User?> GetUserById(string userId)
    {
        var response = await _httpClient.GetAsync($"api/user/{userId}");
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<User>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}