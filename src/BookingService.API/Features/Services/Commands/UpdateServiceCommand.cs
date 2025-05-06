using BookingService.API.Common.Exceptions;
using BookingService.API.Features.Services.Models;
using BookingService.API.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingService.API.Features.Services.Commands;

public sealed class UpdateServiceCommand : IRequest<ServiceResponse>
{
    public UpdateServiceRequest Service {  get; set; }
    public UpdateServiceCommand(UpdateServiceRequest request)
    {
        Service = request;
    }
}

public sealed class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand, ServiceResponse>
{
    private readonly DataContext _context;
    public UpdateServiceCommandHandler(DataContext context)
    {
        _context = context;
    }
    public async Task<ServiceResponse> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await _context.Services
            .FirstOrDefaultAsync(s => s.Id == request.Service.Id, cancellationToken)
            ?? throw new NotFoundException("Service not found");

        // todo: check for room occupation
        var room = await _context.Rooms
            .FirstOrDefaultAsync(r => r.Id == request.Service.RoomId, cancellationToken)
            ?? throw new NotFoundException("Room not found");

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

        request.Service.MapTo(service);
        await _context.SaveChangesAsync(cancellationToken);

        return service.Map();
    }
}
