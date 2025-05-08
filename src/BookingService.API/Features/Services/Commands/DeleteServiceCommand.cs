using BookingService.API.Common.Exceptions;
using BookingService.API.Domain.Enums;
using BookingService.API.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Shared.Clients;
using static ReservationSystem.Shared.ServiceEndpoints;

namespace BookingService.API.Features.Services.Commands;

public sealed class DeleteServiceCommand : IRequest
{
    public int ServiceId { get; set; }
    public DeleteServiceCommand(int serviceId)
    {
        ServiceId = serviceId;
    }
}

public sealed class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand>
{
    private readonly DataContext _context;
    private readonly NetworkHttpClient _client;

    public DeleteServiceCommandHandler(DataContext context, NetworkHttpClient client)
    {
        _context = context;
        _client = client;
    }

    public async Task Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await _context.Services
            .Include(s => s.Bookings)
            .FirstOrDefaultAsync(b => b.Id == request.ServiceId, cancellationToken)
                ?? throw new NotFoundException("Service not found");

        await _context.Bookings
              .Where(b => b.ServiceId == service.Id)
              .ExecuteUpdateAsync(b => b.SetProperty(b => b.Status, BookingStatus.Cancelled), cancellationToken);
        service.IsCancelled = true;

        await _context.SaveChangesAsync(cancellationToken);

        foreach (var booking in service.Bookings)
        {
            await _client.PostAsync<object, object>(NotificationService.SendBookingCancellationEmail, new { BookingId = booking.Id }, CancellationToken.None);
        }
    }
}
