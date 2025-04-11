using FluentValidation;
using NotificationService.API.Persistence.Entities.DB.Models;
using NotificationService.API.Services;
using ReservationSystem.Shared.Results;
using src.NotificationService.API.Persistence.Entities.DB.Models;
using src.NotificationService.API.Services;

namespace NotificationService.API.Features;

public record GetAllNotificationsRequest(int UserId);
public record GetAllNotifictionsResponse(List<NotificationLog>? NotificationLogs);

public class GetAllNotificationsValidator : AbstractValidator<GetAllNotificationsRequest>
{
    public GetAllNotificationsValidator()
    {
        RuleFor(u => u.UserId).GreaterThan(0);
    }
}

public class GetAllNotificationsHandler
{
    private readonly NotificationLogService _notificationLogService;

    public GetAllNotificationsHandler(NotificationLogService notificationLogService)
    {
        _notificationLogService = notificationLogService;
    }

    public async Task<ApiResult<GetAllNotifictionsResponse>> Handle(GetAllNotificationsRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var notifications = await _notificationLogService.GetNotificationsFor(request.UserId);
        bool success = notifications == null || notifications.Count == 0;

        return new ApiResult<GetAllNotifictionsResponse>(
            new GetAllNotifictionsResponse(notifications),
            success,
            notifications?.Count == 0 ? $"No notifications for user id: {request.UserId}" : $"Error while getting notification logs for {request.UserId}"
        );
    }
}

public static class GetAllNotificationsEndpoint
{
    public static void GetAllNotifications(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/notification/logs/getallnotifications/{UserId:int}",
            async (int UserId, GetAllNotificationsHandler handler, GetAllNotificationsValidator validator, CancellationToken cancellationToken) =>
            {
                var request = new GetAllNotificationsRequest(UserId);
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    Console.WriteLine(validationResult.Errors);
                    var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage);
                    return Results.BadRequest(
                        new ApiResult<IEnumerable<string>>(
                            errorMessages,
                            false,
                            "Validation failed"
                        )
                    );
                }

                var result = await handler.Handle(request, cancellationToken);

                return result.Success ? Results.Ok(result) : Results.NotFound(result);
            }
        );
    }
}
