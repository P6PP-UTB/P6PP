using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NotificationService.API.Persistence;
using NotificationService.API.Persistence.Entities.DB;
using NotificationService.API.Persistence.Entities;
using NotificationService.API.Persistence.Entities.DB.Models;

namespace NotificationService.API.Services
{
    public class BookingAppService
    {
        private readonly HttpClient _httpClient;
        private readonly NotificationDbContext _notificationDbContext;
        private readonly UserAppService _userAppService;
        private readonly TemplateAppService _templateAppService;
        private readonly MailAppService _mailAppService;

        public BookingAppService(HttpClient httpClient, NotificationDbContext notificationDbContext,
            UserAppService userAppService,
            TemplateAppService templateAppService, MailAppService mailAppService)
        {
            _httpClient = httpClient;
            _notificationDbContext = notificationDbContext;
            _userAppService = userAppService;
            _templateAppService = templateAppService;
            _mailAppService = mailAppService;

        }

        public async Task<BookingResponse?> GetBookingByIdAsync(int id)
        {
            BookingResponse? response = null;
            try
            {
                var httpResponse = await _httpClient.GetAsync("http://host.docker.internal:8080/api/Bookings/" + id);
                httpResponse.EnsureSuccessStatusCode();
                var content = await httpResponse.Content.ReadAsStringAsync();
                response = JsonSerializer.Deserialize<BookingResponse>(content);
            }
            catch (Exception e)
            {
              //TODO: ADD LOGGING
            }
            return response;
        }

        public async Task<ServiceResponse?> GetServiceByIdAsync(int serviceId)
        {
            ServiceResponse? response = null;
            try
            {
                var httpResponse = await _httpClient.GetAsync("http://host.docker.internal:8080/api/Services/" + serviceId);
                var content = await httpResponse.Content.ReadAsStringAsync();
                response = JsonSerializer.Deserialize<ServiceResponse>(content);
            }
            catch (Exception e)
            {
                //TODO: ADD LOGGING
            }
                
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

        public async Task SaveToTimer(int bookingId, int userId)
        { 
            var booking = await GetBookingByIdAsync(bookingId);
            
            if (booking == null)
            {
                //TODO: ADD LOGGING
                return;
            }
            
            var service = await GetServiceByIdAsync(booking.serviceId);
            if (service == null)
            {
                //TODO: ADD LOGGING
                return;
            }

            var bookingTimer = new Booking()
            {
                BookingId = booking.id,
                Start = service.start,
                UserId = userId,
            };
            var bookingTimerDb = await _notificationDbContext.Bookings.AddAsync(bookingTimer);
        }

        public async Task DeleteTimer(int bookingId)
        {
            var booking = await _notificationDbContext.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking != null)
            {
                _notificationDbContext.Bookings.Remove(booking);
                await _notificationDbContext.SaveChangesAsync();
            }
        }

        public async Task SednReminder24HourBefore()
        {
            var bookings = await _notificationDbContext.Bookings
                .Where(b => b.Start <= DateTime.UtcNow.AddHours(24) && b.Notice24H == false)
                .ToListAsync();
            foreach (var booking in bookings)
            {
                var succes = await SendRemiderEmail(booking);
                if (succes)
                {
                    booking.Notice24H = true;
                    _notificationDbContext.Bookings.Update(booking);
                    await _notificationDbContext.SaveChangesAsync();
                }
            }
           
        }

        public async Task<bool> SendRemiderEmail(Booking bookingLocal)
        {
            var booking = await GetBookingByIdAsync(bookingLocal.BookingId);
            
            if (booking == null || booking.status == "Cancelled")
            {
                //TODO: ADD LOGGING
                return false;
            }
            var service = await GetServiceByIdAsync(booking.serviceId);
      
            if (service == null)
            {
                //TODO: ADD LOGGING
                return false;
            }
            
            var user = await _userAppService.GetUserByIdAsync(bookingLocal.UserId);
            
            if (user == null)
            {
                //TODO: ADD LOGGING
                return false;
            }
            
            var template = await _templateAppService.GetTemplateAsync("BookingConfirmation");
            
            
            template.Text = template.Text.Replace("{name}", user.FirstName + " " + user.LastName);
            template.Text = template.Text.Replace("{eventname}", service.serviceName);
            template.Text = template.Text.Replace("{datetime}", service.start.ToString());

            var emailArgs = new EmailArgs
            {
                Address = new List<string> { user.Email },
                Subject = template.Subject,
                Body = template.Text
            };

            try
            {
                await _mailAppService.SendEmailAsync(emailArgs);
            }
            catch (Exception e)
            {
                //TODO: ADD LOGGING
                return false;
            }
            
            return true;

        }
    }
}