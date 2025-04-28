using FluentValidation;
using ReservationSystem.Shared.Results;
using PaymentService.API.Persistence.Entities.DB.Models;
using PaymentService.API.Services;


namespace PaymentService.API.Features.Payments;

public record CreatePaymentRequest(int UserId, int RoleId, string TransactionType, int Amount);

public class CreatePaymentValidator : AbstractValidator<CreatePaymentRequest>
{
    public CreatePaymentValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("Neplatn� ID u�ivatele.");

        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("Neplatn� ID role.");

        RuleFor(x => x.TransactionType)
            .Must(type => type == "credit" || type == "reservation")
            .WithMessage("TransactionType mus� b�t 'credit' nebo 'reservation'.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("��stka mus� b�t v�t�� ne� 0.");


    }
}

public class CreatePaymentHandler
{
    private readonly PaymentService.API.Services.PaymentService _paymentService;

    public CreatePaymentHandler(PaymentService.API.Services.PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task<ApiResult<int>> HandleAsync(CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();



        if (request.TransactionType == "credit")
        {

            var payment = new Payment
            {
                UserId = request.UserId,
                RoleId = request.RoleId,
                CreditAmount = request.Amount,
                Status = "pending",
                CreatedAt = DateTime.UtcNow
            };
            var id = await _paymentService.CreatePaymentCredits(payment, cancellationToken);
            return id.HasValue
            ? new ApiResult<int>(id.Value)
            : new ApiResult<int>(0, false, "payment not created");
        }
        else
        {
            var payment = new Payment
            {
                UserId = request.UserId,
                RoleId = request.RoleId,
                Price = request.Amount,
                Status = "pending",
                CreatedAt = DateTime.UtcNow

            };
            var id = await _paymentService.CreatePayment(payment, cancellationToken);

            return id.HasValue
            ? new ApiResult<int>(id.Value)
            : new ApiResult<int>(0, false, "payment not created");

        }

    }
}


public static class CreatePaymentEndpoint
{
    public static void Register(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/createpayment",
            async (CreatePaymentRequest request,
                CreatePaymentHandler handler,
                CreatePaymentValidator validator,
                CancellationToken cancellationToken) =>
            {
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage);
                    return Results.BadRequest(new ApiResult<IEnumerable<string>>(errorMessages, false, "Validation failed"));
                }

                var result = await handler.HandleAsync(request, cancellationToken);

                return result.Success
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            });
    }
}