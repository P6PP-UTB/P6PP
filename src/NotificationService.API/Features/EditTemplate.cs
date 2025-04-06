using NotificationService.API.Services;
using ReservationSystem.Shared.Results;
using FluentValidation;
using NotificationService.API.Persistence.Entities.DB.Models;

namespace NotificationService.API.Features;

public record EditTemplateRequest(Template template);
public record EditTemplateResponse(int? id = null);

public class EditTemplateValidator : AbstractValidator<EditTemplateRequest>
{
    public EditTemplateValidator()
    {
        RuleFor(x => x.template.Subject).NotEmpty().MaximumLength(75);
        RuleFor(x => x.template.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.template.Text).NotEmpty().MaximumLength(1500);
    }
}

public class EditTemplateHandler
{
    private readonly TemplateAppService _templateAppService;

    public EditTemplateHandler(TemplateAppService templateAppService)
    {
        _templateAppService = templateAppService;
    }

    public async Task<ApiResult<EditTemplateResponse>> Handle(EditTemplateRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var editSucces = await _templateAppService.EditTemplateAsync(request.template);

        return new ApiResult<EditTemplateResponse>(
            new EditTemplateResponse(),
            editSucces,
            editSucces ? "Template edit saved." : "No template named '{request.template.Name}' in language '{request.template.Language}'!");
    }
}

public static class EditTemplateEndpoint
{
    public static void EditTemplate(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/notification/templates/edittemplate",
            async (EditTemplateRequest request, EditTemplateHandler handler, EditTemplateValidator validator, CancellationToken cancellationToken) =>
            {
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    Console.WriteLine(validationResult.Errors);
                    var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage);
                    return Results.BadRequest(new ApiResult<IEnumerable<string>>(errorMessages, false, "Validation failed"));
                }

                var result = await handler.Handle(request, cancellationToken);

                return result.Success
                    ? Results.Ok(result)
                    : Results.NotFound(result);
            });
    }
}
