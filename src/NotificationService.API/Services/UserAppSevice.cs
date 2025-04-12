using NotificationService.API.Persistence.Entities;
using ReservationSystem.Shared.Clients;
using ReservationSystem.Shared;

namespace NotificationService.API.Services
{
    public class UserAppService
    {
        public record GetUserRespond(User User);

        private readonly NetworkHttpClient _httpClient;

        public UserAppService(NetworkHttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<User?> GetUserByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync<GetUserRespond>(ServiceEndpoints.UserService.GetUserById(id)); 
            Console.WriteLine("User response: " + System.Text.Json.JsonSerializer.Serialize(response));
            return response.Data?.User;
        }
    }
}