
using AdminSettings.Persistence.Entities;

namespace AdminSettings.Services;

public class UserService
{
    private readonly HttpClient _httpClient;
    
    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<User> GetUserById(int id)
    {
        var response = await _httpClient.GetAsync($"api/user/{id}");
        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<User>();
        return user!;
    }
}