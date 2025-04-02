using NotificationService.API.Persistence.Entities;
using ReservationSystem.Shared.Results;
using ReservationSystem.Shared.Clients;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace NotificationService.API.Services
{
    public class UserAppService
    {
        public record GetUserRespond(User user);

        private readonly NetworkHttpClient _httpClient;

        public UserAppService(NetworkHttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<User?> GetUserByIdAsync(int id)
        {
            Console.WriteLine($"GetUserByIdAsync called with id: {id}");
            var response = await _httpClient.GetAsync<ApiResult<GetUserRespond>>($"http://localhost:5189/api/user/{id}");   
            return response.Data?.Data?.user;
            
        }
    }
}