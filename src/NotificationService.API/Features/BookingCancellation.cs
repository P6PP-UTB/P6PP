using NotificationService.API.Persistence;
using NotificationService.API.Services;
using FluentValidation;
using ReservationSystem.Shared.Results;
using src.NotificationService.API.Persistence.Entities.DB.Models;
using src.NotificationService.API.Services;
namespace NotificationService.API.Features;

public record SendBookingCancellationEmailRequest(int UserId, string Name, DateTime Datetime);
//TODO: migrate to BookingId and read property from BookingService
public record SendBookingCancellationEmailResponse(int? Id = null);

public class SendBookingCancellationEmailValidator : AbstractValidator<SendBookingCancellationEmailRequest>
{
    public SendBookingCancellationEmailValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Datetime).NotEmpty();
    }
}

public class SendBookingCancellationEmailHandler
{
    private readonly MailAppService _mailAppService;
    private readonly TemplateAppService _templateAppService;
    private readonly UserAppService _userAppService;
    private readonly NotificationLogService _notificationLogService;

    public SendBookingCancellationEmailHandler(MailAppService mailAppService,
                                               TemplateAppService templateAppService,
                                               UserAppService userAppService,
                                               NotificationLogService notificationLogService
    )
    {
        _mailAppService = mailAppService;
        _templateAppService = templateAppService;
        _userAppService = userAppService;
        _notificationLogService = notificationLogService;
    }

    public async Task<ApiResult<SendBookingCancellationEmailResponse>> Handle(SendBookingCancellationEmailRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await _userAppService.GetUserByIdAsync(request.UserId);

        if (user == null)
        {
            return new ApiResult<SendBookingCancellationEmailResponse>(null, false, "User not found");
        }
        if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName))
        {
            return new ApiResult<SendBookingCancellationEmailResponse>(null, false, "User email or name not found");
        }

        var template = await _templateAppService.GetTemplateAsync("BookingCancellation");

        template.Text = template.Text.Replace("{name}", user.FirstName + " " + user.LastName);
        template.Text = template.Text.Replace("{eventname}", request.Name);
        template.Text = template.Text.Replace("{datetime}", request.Datetime.ToString());

        var emailArgs = new EmailArgs
        {
            Address = new List<string> { user.Email },
            Subject = template.Subject,
            Body = template.Text
        };

        try
        {
            await _mailAppService.SendEmailAsync(emailArgs);
            await _notificationLogService.LogNotification(user.Id,
                                                          NotificationType.ReservationCanceled,
                                                          template.Subject,
                                                          template.Text
            );
            return new ApiResult<SendBookingCancellationEmailResponse>(new SendBookingCancellationEmailResponse());
        }
        catch
        {
            return new ApiResult<SendBookingCancellationEmailResponse>(null, false, "Email was not sent");
        }
    }
}
public static class SendBookingCancellationEmailEndpoint
{
    public static void SendBookingCancellationEmail(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/notification/booking/sendbookingcancellationemail",
            async (SendBookingCancellationEmailRequest request, SendBookingCancellationEmailHandler handler, SendBookingCancellationEmailValidator validator, CancellationToken cancellationToken) =>
            {
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage);
                    return Results.BadRequest(new ApiResult<IEnumerable<string>>(errorMessages, false, "Validation failed"));
                }

                var result = await handler.Handle(request, cancellationToken);

                return result.Success
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            });
    }
}
