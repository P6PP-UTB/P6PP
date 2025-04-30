using BookingService.API.Common.Exceptions;
using BookingService.API.Features.Services.Models;
using BookingService.API.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingService.API.Features.Services.Commands;

public sealed class CreateServiceCommand : IRequest<ServiceResponse>
{
    public CreateServiceRequest Service;
    public CreateServiceCommand(CreateServiceRequest service)
    {
        Service = service;
    }
}

public sealed class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, ServiceResponse>
{
    private readonly DataContext _context;
    public CreateServiceCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var room = await _context.Rooms
            .FirstOrDefaultAsync(r => r.Id == request.Service.RoomId, cancellationToken)
            ?? throw new NotFoundException("Room not found");

        // todo: get trainer id from token
        // todo: check if trainer has other services during start-end
        var service = request.Service.Map();
        service.RoomId = room.Id;

        var overlappingServicesRoom = await _context.Services
            .Where(s =>
                s.RoomId == request.Service.RoomId &&
                !s.IsCancelled &&
                s.Start < request.Service.End &&
                s.End > request.Service.Start)
            .ToListAsync(cancellationToken);

        if (overlappingServicesRoom.Any())
        {
            throw new ValidationException("Room is occupied during the selected time");
        }

        _context.Services.Add(service);
        await _context.SaveChangesAsync(cancellationToken);

        return service.Map();
    }
}
