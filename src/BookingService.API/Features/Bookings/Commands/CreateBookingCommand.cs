﻿using BookingService.API.Common.Exceptions;
using BookingService.API.Common.MediatR.Interfaces;
using BookingService.API.Domain.Enums;
using BookingService.API.Features.Bookings.Models;
using BookingService.API.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Shared.Clients;
using static ReservationSystem.Shared.ServiceEndpoints;

namespace BookingService.API.Features.Bookings.Commands;

public sealed class CreateBookingCommand : IRequest<BookingResponse>, IRequestWithUserId
{
    public CreateBookingCommand(CreateBookingRequest booking)
    {
        Booking = booking;
    }

    public CreateBookingRequest Booking { get; set; }
    public int UserId { get; set; }
}

public sealed class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingResponse>
{
    private readonly DataContext _context;
    private readonly NetworkHttpClient _client;

    public CreateBookingCommandHandler(DataContext context, NetworkHttpClient client)
    {
        _context = context;
        _client = client;
    }

    public async Task<BookingResponse> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        // todo: check if user has money
        var service = await _context.Services
                    .Include(s => s.Room)
                    .FirstOrDefaultAsync(s => s.Id == request.Booking.ServiceId, cancellationToken)
                    ?? throw new NotFoundException("Service not found");

        if (service.IsCancelled)
            throw new ValidationException("Service is cancelled");

        if (service.Users.Count >= service.Room!.Capacity)
            throw new ValidationException("Service capacity is full");

        var alreadyRegistered = await _context.Bookings
            .AnyAsync(b => b.UserId == request.UserId && b.ServiceId == request.Booking.ServiceId, cancellationToken);

        if (alreadyRegistered)
            throw new ValidationException("User already registered on this service");

        service.Users!.Add(request.UserId);

        var booking = request.Booking.Map();
        booking.BookingDate = DateTime.UtcNow;
        booking.UserId = request.UserId;
        booking.Status = BookingStatus.Pending;

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync(cancellationToken);

        await _client.PostAsync<object, object>(NotificationService.SendBookingConfirmationEmail, new { BookingId = booking.Id }, CancellationToken.None);

        return booking.Map();
    }
}
