using NotificationService.API.Persistence;
using NotificationService.API.Services;
using FluentValidation;
using ReservationSystem.Shared.Results;
namespace NotificationService.API.Features;

public record SendBookingConfirmationEmailRequest(int UserId, string Name, DateTime Datetime);
//TODO: migrate to BookingId and read property from BookingService
public record SendBookingConfirmationEmailResponse(int? Id = null);

public class SendBookingConfirmationEmailValidator : AbstractValidator<SendBookingConfirmationEmailRequest>
{
    public SendBookingConfirmationEmailValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Datetime).NotEmpty();
    }
}

public class SendBookingConfirmationEmailHandler
{
    private readonly MailAppService _mailAppService;
    private readonly TemplateAppService _templateAppService;
    private readonly UserAppService _userAppService;

    public SendBookingConfirmationEmailHandler(MailAppService mailAppService,
        TemplateAppService templateAppService, UserAppService userAppService)
    {
        _mailAppService = mailAppService;
        _templateAppService = templateAppService;
        _userAppService = userAppService;
    }

    public async Task<ApiResult<SendBookingConfirmationEmailResponse>> Handle(SendBookingConfirmationEmailRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await _userAppService.GetUserByIdAsync(request.UserId);

        if (user == null)
        {
            return new ApiResult<SendBookingConfirmationEmailResponse>(null, false, "User not found");
        }
        if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName))
        {
            return new ApiResult<SendBookingConfirmationEmailResponse>(null, false, "User email or name not found");
        }

        var template = await _templateAppService.GetTemplateAsync("BookingConfirmation");

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
            return new ApiResult<SendBookingConfirmationEmailResponse>(new SendBookingConfirmationEmailResponse());
        }
        catch (Exception e)
        {
            return new ApiResult<SendBookingConfirmationEmailResponse>(null, false, "Email was not sent: " + e);
        }
    }
}

public static class SendBookingConfirmationEmailEndpoint
{
    public static void SendBookingConfirmationEmail(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/notification/booking/sendbookingconfirmationemail",
            async (SendBookingConfirmationEmailRequest request, SendBookingConfirmationEmailHandler handler, SendBookingConfirmationEmailValidator validator, CancellationToken cancellationToken) =>
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