using BookingService.API.Common.Exceptions;
using BookingService.API.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Shared.Clients;
using static ReservationSystem.Shared.ServiceEndpoints;

namespace BookingService.API.Features.Bookings.Commands;

public sealed class DeleteBookingCommand : IRequest
{
    public DeleteBookingCommand(int bookingId)
    {
        BookingId = bookingId;
    }

    public int BookingId { get; set; }
}

public sealed class DeleteBookingCommandHandler : IRequestHandler<DeleteBookingCommand>
{
    private readonly DataContext _context;
    private readonly NetworkHttpClient _client;

    public DeleteBookingCommandHandler(DataContext context, NetworkHttpClient client)
    {
        _context = context;
        _client = client;
    }

    public async Task Handle(DeleteBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken)
            ?? throw new NotFoundException("Booking not found");

        var service = await _context.Services
            .FirstOrDefaultAsync(s => s.Id == booking.ServiceId, cancellationToken)
            ?? throw new NotFoundException("Service not found");

        service.Users.Remove(booking.UserId);
        // todo: check ownership?
        // todo: payments etc.

        await _client.PostAsync<object, object>(NotificationService.SendBookingCancellationEmail, new { BookingId = booking.Id }, CancellationToken.None);
        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
