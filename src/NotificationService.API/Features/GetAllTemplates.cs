using NotificationService.API.Services;
using ReservationSystem.Shared.Results;
using NotificationService.API.Persistence.Entities.DB.Models;

namespace NotificationService.API.Features;

public record GetAllTemplatesResponse(List<Template> templates);

public class GetAllTemplatesHandler
{
    private readonly TemplateAppService _templateAppService;

    public GetAllTemplatesHandler(TemplateAppService templateAppService)
    {
        _templateAppService = templateAppService;
    }

    public async Task<ApiResult<GetAllTemplatesResponse>> Handle(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var templates = await _templateAppService.GetAllTemplatesAsync();

        if (templates == null || templates.Count == 0)
        {
            return new ApiResult<GetAllTemplatesResponse>(null, false, "No templates in database");
        }
        return new ApiResult<GetAllTemplatesResponse>(new GetAllTemplatesResponse(templates));
    }
}

public static class GetAllTemplatesEndpoint
{
    public static void GetAllTemplates(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/notification/templates/getalltemplates",
            async (GetAllTemplatesHandler handler, CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(cancellationToken);

                return result.Success
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            });
    }
}
