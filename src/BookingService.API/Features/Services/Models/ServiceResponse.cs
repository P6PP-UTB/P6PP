namespace BookingService.API.Features.Services.Models;

public record ServiceResponse(
    int Id,
    int TrainerId,
    DateTime Start,
    DateTime End,
    int Price,
    string ServiceName,
    int CurrentCapacity,
    int TotalCapacity,
    string RoomName,
    bool IsCancelled);
