using System.Text.Json;
using NotificationService.API.Persistence.Entities;
using ReservationSystem.Shared.Clients;
using ReservationSystem.Shared;
using ReservationSystem.Shared.Results;

namespace NotificationService.API.Services
{
    public class BookingAppService
    {
        private readonly HttpClient _httpClient;

        public BookingAppService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<BookingResponse?> GetBookingByIdAsync(int id)
        {
            // TODO: Use shared http client and api call  - Waiting form Booking team
            var httpResponse = await _httpClient.GetAsync("http://host.docker.internal:8080/api/Bookings/" + id);
            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<BookingResponse>(content);
            return response;
        }

        public async Task<ServiceResponse?> GetServiceByIdAsync(int serviceId)
        {
            // TODO: Use shared http client and api call  - Waiting form Booking team
            var httpResponse = await _httpClient.GetAsync("http://host.docker.internal:8080/api/Services/" + serviceId);
            var content = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<ServiceResponse>(content);
            return response;
        }
        
        public async Task<ServiceResponse?> GetServiceByBookingIdAsync(int bookingId)
        {
            var booking = await GetBookingByIdAsync(bookingId);
            if (booking == null)
            {
                return null;
            }
            var service = await GetServiceByIdAsync(booking.serviceId);
            return service;
        }
    }
}